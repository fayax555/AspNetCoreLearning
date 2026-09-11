const descriptionInput = document.querySelector('#Description');

const quill = new Quill("#description-editor", {
    theme: "snow",
    modules: {
        toolbar: [
            ["bold", "italic", "underline"],
            ["clean"]
        ]
    }
});

if (descriptionInput.value) {
    quill.clipboard.dangerouslyPasteHTML(descriptionInput.value);
}

quill.on("text-change", () => {
    const descriptionHasText = quill.getText().trim().length > 0;
    descriptionInput.value = descriptionHasText ? quill.getSemanticHTML() : "";
});