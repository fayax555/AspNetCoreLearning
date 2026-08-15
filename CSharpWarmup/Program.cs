namespace CSharpWarmup
{
    class Program
    {
        static void Main()
        {
            List<Todo> tasks = new List<Todo>();

            while (true)
            {
                string? taskTitle = ReadTaskTitle();

                if (taskTitle == null)
                {
                    if (tasks.Count < 1)
                    {
                        Console.WriteLine("You did not have any tasks");
                        break;
                    }

                    PrintTasks(tasks);

                    break;
                }

                if (string.Equals(taskTitle, "complete", StringComparison.OrdinalIgnoreCase))
                {
                    CompleteTask(tasks);
                    continue;
                }

                TodoPriority priority;

                if (!TryReadPriority(out priority))
                {
                    continue;
                }

                Todo newTask = new Todo(taskTitle, priority);
                tasks.Add(newTask);

                Console.WriteLine($"Task added: {taskTitle} - Priority {priority}");
                Console.WriteLine();
            }
        }

        static void CompleteTask(IEnumerable<Todo> tasks)
        {
            var orderedTasks = GetOrderedTasks(tasks);

            if (orderedTasks.Count == 0)
            {
                Console.WriteLine("There are no tasks to complete");
                return;
            }

            PrintTasks(orderedTasks);

            Console.Write("Enter a number to complete: ");

            if (!int.TryParse(Console.ReadLine(), out int enteredNumber))
            {
                Console.WriteLine("Please enter a valid number.");
                return;
            }

            if (enteredNumber < 1 || enteredNumber > orderedTasks.Count)
            {
                Console.WriteLine($"Please enter a number between 1 and {orderedTasks.Count}\n");
                return;
            }

            var task = orderedTasks[enteredNumber - 1];

            if (task.IsCompleted)
            {
                Console.WriteLine("That task is already completed\n");
                return;
            }

            task.MarkCompleted();

            Console.WriteLine($"Completed: {task.Title}\n");
        }

        static string? ReadTaskTitle()
        {
            while (true)
            {
                Console.Write("Enter a task title, or type complete or quit: ");

                string? taskTitle = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(taskTitle))
                {
                    Console.WriteLine("Please enter a valid task title\n");
                    continue;
                }

                taskTitle = taskTitle.Trim();

                if (string.Equals(taskTitle, "quit", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                return taskTitle;
            }
        }

        static bool TryReadPriority(out TodoPriority priority)
        {
            priority = default;

            Console.Write("Enter priority (1-3): ");
            string? priorityStr = Console.ReadLine();

            TodoPriority todoPriority;

            if (int.TryParse(priorityStr, out int result))
            {
                todoPriority = (TodoPriority)result;
            }
            else
            {
                Console.WriteLine("Please enter a whole number between 1 and 3\n");
                return false;
            }

            if (!Enum.IsDefined(todoPriority))
            {
                Console.WriteLine("Priority must be between 1 and 3\n");
                return false;
            }

            priority = todoPriority;
            return true;
        }

        static void PrintTasks(IEnumerable<Todo> tasks)
        {
            Console.WriteLine("\nYour tasks: ");

            int index = 1;
            foreach (var task in GetOrderedTasks(tasks))
            {
                Console.WriteLine($"[{(task.IsCompleted ? "x" : " ")}] {index}. {task.Title} - Priority {task.Priority}.");
                index++;
            }
                
            Console.WriteLine($"\nIncomplete high-priority tasks: {tasks.Count(task => task.Priority == TodoPriority.High && !task.IsCompleted)}\n");
        }

        static List<Todo> GetOrderedTasks(IEnumerable<Todo> tasks)
        {
            return tasks.OrderBy(task => task.Priority).ThenBy(task => task.Title).ToList();
        }
    }

}