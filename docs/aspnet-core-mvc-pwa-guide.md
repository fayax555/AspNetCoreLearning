# Adding PWA support to an ASP.NET Core MVC application

Last reviewed: August 19, 2026  
Example target: ASP.NET Core MVC on .NET 9

This guide explains how to make a server-rendered ASP.NET Core MVC application installable as a Progressive Web App (PWA), give it an app name and icons, run it in a standalone window, and provide a safe offline fallback page.

The guide deliberately does **not** pretend that database-backed MVC forms work offline. Offline create, edit, and delete operations require a separate synchronization design, discussed near the end.

## What changes when an MVC application becomes a PWA?

The ASP.NET Core application still runs on the server:

- Controllers still handle requests.
- Razor views still generate HTML.
- Entity Framework Core still talks to the database.
- Form submissions still require the server unless an offline synchronization system is explicitly built.

PWA support adds browser-side capabilities:

| Part | Responsibility |
| --- | --- |
| Web app manifest | Describes the installed app's name, icon, start page, colors, and display mode. |
| Service worker | Runs separately from the page and can intercept requests, use caches, show offline content, receive push events, and perform supported background work. |
| HTTPS | Protects the service worker and other powerful browser APIs from network tampering. `localhost` is treated as secure for development. |
| Stable origin | Gives the installed app a stable identity. An origin is the scheme, host, and port, such as `https://todos.example.com`. |

An MVC application does not need to become a Single Page Application (SPA) to be a PWA.

## The implementation used in this guide

This guide builds the PWA in two levels:

1. **Installable application**
   - Web app manifest
   - App icons
   - Standalone display
   - HTTPS URL
2. **Safe offline fallback**
   - Service worker
   - Static offline page
   - Network-first MVC navigation
   - No caching of form submissions or database-backed HTML

This is a safe starting point for a traditional MVC application.

## Prerequisites

- An ASP.NET Core MVC application.
- Static assets served from `wwwroot`.
- HTTPS in production.
- A current browser with PWA support.
- Square PNG app artwork.

In a .NET 9 MVC application, static assets can be exposed with `MapStaticAssets`:

```csharp
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
```

The example project already has this in [`MvcStarter/Program.cs`](../MvcStarter/Program.cs).

Files under `wwwroot` become public URLs. For example:

| Project file | Public URL |
| --- | --- |
| `wwwroot/manifest.json` | `/manifest.json` |
| `wwwroot/service-worker.js` | `/service-worker.js` |
| `wwwroot/offline.html` | `/offline.html` |
| `wwwroot/icons/icon-192.png` | `/icons/icon-192.png` |

Do not place secrets, source files, or private configuration in `wwwroot`.

## Recommended file structure

```text
MvcStarter/
├── Views/
│   └── Shared/
│       └── _Layout.cshtml
└── wwwroot/
    ├── icons/
    │   ├── icon-192.png
    │   └── icon-512.png
    ├── js/
    │   └── site.js
    ├── manifest.json
    ├── offline.html
    └── service-worker.js
```

## Step 1: create the app icons

At minimum, provide:

- `wwwroot/icons/icon-192.png`, exactly 192 by 192 pixels.
- `wwwroot/icons/icon-512.png`, exactly 512 by 512 pixels.

Use real PNG files. Renaming a JPEG or changing a filename does not resize or convert the image.

Keep important artwork away from the edges because operating systems may crop icons into circles, rounded squares, or other shapes.

For a production application, also consider a separately designed maskable icon:

```json
{
  "src": "/icons/icon-maskable-512.png",
  "sizes": "512x512",
  "type": "image/png",
  "purpose": "maskable"
}
```

Do not label an icon as `maskable` unless its artwork was designed with the maskable safe area in mind.

For iOS, current Safari versions can use icons from the manifest when their purpose includes `any`. A product team can also supply an Apple-specific icon:

```html
<link rel="apple-touch-icon" href="~/icons/apple-touch-icon.png" />
```

When both are present, Apple can prefer `apple-touch-icon` over the manifest icon.

## Step 2: create the web app manifest

Create `wwwroot/manifest.json`:

```json
{
  "id": "/",
  "name": "MvcStarter Todos",
  "short_name": "Todos",
  "description": "A todo management application",
  "start_url": "/Todos",
  "scope": "/",
  "display": "standalone",
  "background_color": "#ffffff",
  "theme_color": "#0d6efd",
  "icons": [
    {
      "src": "/icons/icon-192.png",
      "sizes": "192x192",
      "type": "image/png",
      "purpose": "any"
    },
    {
      "src": "/icons/icon-512.png",
      "sizes": "512x512",
      "type": "image/png",
      "purpose": "any"
    }
  ]
}
```

### What the manifest properties mean

| Property | Meaning |
| --- | --- |
| `id` | Stable identity for the installed app. Avoid changing it after release. |
| `name` | Full application name used by installation UI and operating-system surfaces. |
| `short_name` | Shorter name used when screen space is limited. |
| `description` | Human-readable description of the app. |
| `start_url` | URL opened from the installed app icon. |
| `scope` | URLs treated as part of the installed application. |
| `display` | `standalone` requests an app-like window without the normal browser address bar. |
| `background_color` | Color browsers may use while the application starts. |
| `theme_color` | Color used by supported browser and operating-system UI. |
| `icons` | Images used on home screens, launchers, task switchers, and installation UI. |

### Important URL rules

The example uses root-relative URLs such as `/Todos` and `/icons/icon-192.png`. This is appropriate when the application owns the root of its origin.

If the application is hosted under a path such as `https://example.com/todos-app/`, review every root-relative URL. The `id`, `start_url`, `scope`, icons, service-worker registration, and offline URL must all match the deployment path.

The `start_url` must remain on the same origin as the manifest.

The filename `manifest.json` is conventional, not mandatory. JSON is convenient because most servers already return it as `application/json`.

## Step 3: link the manifest from the MVC layout

Add the following inside `<head>` in `Views/Shared/_Layout.cshtml`:

```html
<link rel="manifest" href="~/manifest.json" />
<meta name="theme-color" content="#0d6efd" />
```

The shared layout is the right location because every installable page should reference the same manifest.

At this point, the application has enough metadata for browser menu-based installation on current Chromium browsers. A service worker is still valuable for a controlled offline experience and other PWA features.

## Step 4: verify the manifest before adding a service worker

Run the application and check these URLs directly:

```text
https://your-host/manifest.json
https://your-host/icons/icon-192.png
https://your-host/icons/icon-512.png
```

Expected results:

- The manifest returns HTTP 200 and valid JSON.
- The manifest response type is `application/json` or another valid manifest JSON type.
- Both icons return HTTP 200 with `image/png`.
- The 192 icon is really 192 by 192 pixels.
- The 512 icon is really 512 by 512 pixels.

In Edge or Chrome DevTools:

1. Open the application.
2. Press `F12`.
3. Select **Application**.
4. Select **Manifest**.
5. Confirm the browser shows the name, start URL, colors, and icons without errors.

Do not rely on an old tutorial that requires a Lighthouse "PWA badge." Chrome removed the dedicated Lighthouse PWA category. Use the Application tool and an actual installation test.

## Step 5: create a static offline page

Create `wwwroot/offline.html`.

Keep this page self-contained. If its CSS, fonts, or images require the network, the fallback page may itself look broken while offline.

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="theme-color" content="#0d6efd" />
    <title>Offline - Todos</title>
    <style>
        body {
            margin: 0;
            min-height: 100vh;
            display: grid;
            place-items: center;
            padding: 1.5rem;
            font-family: system-ui, sans-serif;
            text-align: center;
            color: #212529;
            background: #f8f9fa;
        }

        main {
            max-width: 32rem;
        }

        h1 {
            color: #0d6efd;
        }
    </style>
</head>
<body>
    <main>
        <h1>You are offline</h1>
        <p>Reconnect to the internet, then reload the application.</p>
    </main>
</body>
</html>
```

This is a static file rather than a Razor view because the server is unavailable when the device is offline.

## Step 6: create a safe service worker for MVC navigation

Create `wwwroot/service-worker.js`:

```javascript
const CACHE_NAME = "mvcstarter-offline-v1";
const OFFLINE_URL = "/offline.html";

self.addEventListener("install", event => {
    event.waitUntil(
        caches
            .open(CACHE_NAME)
            .then(cache => cache.add(OFFLINE_URL))
    );
});

self.addEventListener("activate", event => {
    event.waitUntil(
        caches
            .keys()
            .then(cacheNames =>
                Promise.all(
                    cacheNames
                        .filter(cacheName =>
                            cacheName.startsWith("mvcstarter-") &&
                            cacheName !== CACHE_NAME
                        )
                        .map(cacheName => caches.delete(cacheName))
                )
            )
    );
});

self.addEventListener("fetch", event => {
    const request = event.request;

    if (request.method !== "GET" || request.mode !== "navigate") {
        return;
    }

    event.respondWith(
        fetch(request)
            .catch(() => caches.match(OFFLINE_URL))
    );
});
```

### Why this service worker is deliberately limited

The `install` event stores only the static offline page.

The `activate` event deletes older caches owned by this application. It checks the `mvcstarter-` prefix so it does not blindly delete unrelated caches on the same origin.

The `fetch` event handles only:

- `GET` requests
- Full-page navigations

It tries the network first. Only a network failure produces the offline page.

It does **not** intercept:

- Form `POST` requests
- Create, edit, complete, or delete operations
- JSON API calls
- Images, scripts, or stylesheets
- Database queries

This prevents a simple PWA experiment from claiming that data was saved when the server never received it.

It also avoids caching server-rendered pages that may contain:

- Stale database results
- User-specific information
- Authorization-sensitive content
- Antiforgery tokens
- Validation errors from an earlier request

## Step 7: register the service worker

Add this to `wwwroot/js/site.js`:

```javascript
if ("serviceWorker" in navigator) {
    window.addEventListener("load", () => {
        navigator.serviceWorker
            .register("/service-worker.js")
            .then(registration => {
                console.log(
                    "Service worker registered with scope:",
                    registration.scope
                );
            })
            .catch(error => {
                console.error("Service worker registration failed:", error);
            });
    });
}
```

Registration returns a JavaScript `Promise` because the browser performs it asynchronously.

### Why the worker is at the root of `wwwroot`

The worker's default scope is based on the directory containing the worker file.

- `/service-worker.js` can control `/` and child URLs such as `/Todos`.
- `/js/service-worker.js` would normally control only `/js/` and its children.

Keep the worker at `/service-worker.js` when it must handle the whole MVC application.

## Step 8: test the service worker in DevTools

After saving the files:

1. Run the application over HTTPS or `localhost`.
2. Open DevTools with `F12`.
3. Select **Application > Service workers**.
4. Confirm `/service-worker.js` is registered.
5. Confirm its status becomes activated.
6. Reload or navigate once so the page is controlled by the active worker.
7. Enable the **Offline** checkbox.
8. Navigate to another MVC page or reload.
9. Confirm `offline.html` appears.
10. Disable **Offline** and reload to return to the application.

Also inspect **Application > Cache storage**. The cache should contain the offline page and should not contain todo pages or submitted form responses.

If the service worker does not appear:

- Open the Console and look for the registration error.
- Request `/service-worker.js` directly and confirm it returns HTTP 200.
- Confirm its response is JavaScript, not an HTML error page.
- Confirm the page is using HTTPS or `localhost`.
- Confirm the registration URL starts at `/service-worker.js`.

## Step 9: test on a phone with a Visual Studio dev tunnel

A dev tunnel exposes the locally running ASP.NET Core application through an HTTPS URL.

In Visual Studio:

1. Set the MVC project as the startup project.
2. Open the dropdown beside the launch profile.
3. Select **Dev Tunnels > Create A Tunnel**.
4. Choose **Persistent** when testing installation.
5. Choose the required access level.
6. Make the tunnel active.
7. Run the application.

Use a persistent tunnel because an installed PWA is associated with its origin. A temporary tunnel produces a different hostname after Visual Studio restarts, leaving the installed test app pointed at the old URL.

Persistent does not mean hosted. Visual Studio and the ASP.NET Core application must still be running.

### Tunnel access safety

- **Private** is safer and requires authorized access.
- **Public** is convenient for a disposable test application.
- Never expose work data, credentials, administrative functionality, or an unauthenticated production-like system through a public tunnel.
- Remove the tunnel after testing if it is no longer needed.

### Android installation

In Chrome or another supporting browser:

1. Open the tunnel URL.
2. Continue past the dev-tunnel warning if it appears.
3. Open the browser menu.
4. Select **Install app** or **Add to Home screen**.
5. Launch the new icon from the home screen.

Expected results:

- The installed name comes from `name` or `short_name`.
- The app uses a manifest icon.
- The app opens at `start_url`.
- Standalone mode does not show the normal browser address bar.

Current Chrome versions allow menu-based installation without a service-worker fetch handler. Automatic install promotion has additional browser heuristics, so the absence of an automatic banner is not by itself a failure.

### iPhone and iPad installation

In Safari:

1. Open the tunnel URL.
2. Tap **Share**.
3. Select **Add to Home Screen**.
4. Keep **Open as Web App** enabled when that option is shown.
5. Add and launch the icon.

Current iOS and iPadOS versions can add websites as Home Screen web apps. A valid manifest still supplies useful metadata such as app identity, colors, start URL, display preferences, and icons.

## Updating the PWA safely

The browser periodically checks whether the service-worker file changed byte-for-byte.

When changing cached resources:

1. Update the resource.
2. Change the cache version:

```javascript
const CACHE_NAME = "mvcstarter-offline-v2";
```

3. Deploy the new service worker at the same `/service-worker.js` URL.
4. Let the normal service-worker lifecycle install and activate the update.

Avoid naming workers `service-worker-v1.js`, `service-worker-v2.js`, and so on. Keep one stable registration URL and change its contents.

By default, a newly installed worker may wait until pages using the old worker are closed. This avoids one page using two incompatible application versions.

`self.skipWaiting()` and `clients.claim()` can activate an update more aggressively, but they can also make a new worker control a page loaded with older HTML and JavaScript. Use them only after defining an update strategy and testing mixed-version behavior.

During development, DevTools offers:

- **Update on reload** to check for a worker update on every reload.
- **Bypass for network** to temporarily ignore the worker.
- **Update** to request an update.
- **Unregister** to remove the worker registration.
- **Storage > Clear site data** to remove registrations, caches, and browser storage.

If manifest metadata or icons appear stale, uninstall the PWA, clear site data, reload the site, and reinstall it.

## Production checklist

### Hosting and origin

- Use a real HTTPS host with a stable hostname.
- Treat the origin as part of the app's permanent identity.
- Do not use a Visual Studio dev tunnel as production hosting.
- Verify manifest, icons, and worker URLs after deployment.
- If using a reverse proxy or virtual application path, validate all scopes and root-relative URLs.

### Authentication and authorization

- A PWA does not add authentication.
- Protect MVC actions exactly as you would for a normal website.
- Treat the installed app as another browser client, not as a trusted native executable.
- Do not cache private pages unless the security and logout behavior have been carefully designed.

### Caching

- Give every cache a versioned name.
- Delete only caches owned by the application.
- Do not cache `POST`, `PUT`, `PATCH`, or `DELETE` responses as if mutations succeeded.
- Be cautious with pages containing personal information or antiforgery tokens.
- Test upgrades while an older installed PWA is open.
- Test logout and account switching with caches present.

### Database and backend

Installing the PWA does not move the database onto the phone. The installed app still contacts the ASP.NET Core server.

For the example project, SQLite is appropriate for local learning and a single-user demo. Before production deployment, decide:

- Where the database file or managed database lives.
- How migrations are applied.
- Whether the host has durable storage.
- Whether multiple application instances will access the database.
- How backups, restore, concurrency, and secrets are handled.

For a multi-user work system, a managed database such as Azure SQL or PostgreSQL is usually a more appropriate design than a SQLite file deployed with the web application.

### Cross-browser testing

Test on the actual supported devices and browser versions. PWA installation UI and optional capabilities differ between:

- Chrome on Android
- Samsung Internet
- Edge on Windows
- Safari on iPhone and iPad
- Managed enterprise devices

Do not assume that one successful desktop installation proves phone support.

## What full offline editing would require

The offline fallback in this guide does not make todo operations available offline.

Real offline editing normally requires:

1. A client-side data store such as IndexedDB.
2. Client-side UI capable of rendering and modifying that local data.
3. A queue of pending changes.
4. An API designed for synchronization.
5. Stable record identifiers that can be created offline.
6. Retry and authentication-expiration handling.
7. A conflict policy for simultaneous edits.
8. User-visible states such as pending, synchronized, and failed.
9. Testing for duplicated, reordered, or partially completed requests.

Background Sync can help on supporting browsers, but browser support and operating-system scheduling vary. The application must still work when background sync is unavailable.

For a traditional server-rendered MVC application, this is a separate architectural feature—not a small addition to the service worker.

## Troubleshooting table

| Symptom | Things to check |
| --- | --- |
| Manifest is missing in DevTools | Confirm the `<link rel="manifest">` appears in the rendered HTML and `/manifest.json` returns valid JSON. |
| Icons are missing | Open each icon URL directly; check HTTP status, MIME type, dimensions, manifest paths, and JSON syntax. |
| Install option is missing | Check HTTPS, manifest fields, icons, whether the app is already installed, browser support, and browser engagement rules. |
| Wrong or old icon appears | Uninstall the app, clear site data, confirm the new manifest response, and reinstall. Check for an overriding `apple-touch-icon` on iOS. |
| Worker registration fails | Check HTTPS, script URL, JavaScript MIME type, syntax errors, and worker scope. |
| Worker controls only `/js/` | Move it to `/service-worker.js` or deliberately configure a broader scope and the required response header. |
| Offline reload shows the browser error page | Confirm the worker is activated and controlling the page, and confirm `offline.html` exists in Cache storage. |
| Changes appear stale | Use Update on reload or Bypass for network, update the cache name, unregister the worker, or clear site data. |
| Tunnel URL is unavailable | Confirm Visual Studio and the application are running and the tunnel is active. |
| Installed test app points to an old tunnel | Uninstall it and reinstall from the persistent or production origin. |

## Verification checklist

- [ ] Project builds without errors.
- [ ] Application is available through HTTPS or `localhost`.
- [ ] Every MVC page links the manifest through the shared layout.
- [ ] Manifest is valid JSON and returns HTTP 200.
- [ ] Manifest contains a stable `id`.
- [ ] Manifest contains `name` or `short_name`.
- [ ] Manifest contains `start_url`, `scope`, and `display`.
- [ ] 192 and 512 PNG icons return HTTP 200.
- [ ] Browser DevTools detects the manifest without errors.
- [ ] Installation works on each supported device family.
- [ ] Installed app opens at the intended page in standalone mode.
- [ ] Service worker registers at the intended scope.
- [ ] Offline fallback appears when navigation loses the network.
- [ ] Form mutations are not falsely reported as successful while offline.
- [ ] Updating the cache version removes old application caches.
- [ ] Production host uses a stable HTTPS origin.
- [ ] Authentication, data storage, migrations, and backups are handled separately.

## References

- [Get started developing a PWA - Microsoft Edge](https://learn.microsoft.com/en-us/microsoft-edge/progressive-web-apps/how-to/)
- [Debug a PWA with the Application tool - Microsoft Edge](https://learn.microsoft.com/en-us/microsoft-edge/devtools/progressive-web-apps/)
- [Dev tunnels in Visual Studio](https://learn.microsoft.com/en-us/aspnet/core/test/dev-tunnels?view=aspnetcore-9.0)
- [Static files in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-9.0)
- [Chrome installability criteria update](https://developer.chrome.com/blog/update-install-criteria)
- [ServiceWorkerContainer.register() and scope - MDN](https://developer.mozilla.org/en-US/docs/Web/API/ServiceWorkerContainer/register)
- [Service worker lifecycle - web.dev](https://web.dev/articles/service-worker-lifecycle)
- [WebKit features in Safari 26](https://webkit.org/blog/17333/webkit-features-in-safari-26-0/)
- [Web Push and Home Screen web apps on iOS and iPadOS](https://webkit.org/blog/13878/web-push-for-web-apps-on-ios-and-ipados/)

