using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace LabRab16
{
    class Program
    {
        static void Main(string[] args)
        {
            //16.1 и 16.2
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            Console.WriteLine("N = 7000");

            Task task = new Task(() =>      //Решето Эратосфена
            {
                Console.WriteLine($"\nИдентификатор - {Task.CurrentId}");
                Console.WriteLine("\n");
                int n = 7000;  //Число
                var numbers = new List<uint>();
                //заполнение списка числами от 2 до n-1
                for (var i = 2u; i < n; i++)
                {
                    numbers.Add(i);
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Операция прервана");
                        return;
                    }
                }
                for (var i = 0; i < numbers.Count; i++)
                {
                    for (var j = 2u; j < n; j++)
                    {
                        //удаляем кратные числа из списка
                        numbers.Remove(numbers[i] * j);
                        if (token.IsCancellationRequested)
                        {
                            Console.WriteLine("Операция прервана");
                            return;
                        }
                    }
                }
                Console.WriteLine(string.Join(", ", numbers));
            });
            task.Start();
            Console.WriteLine($"Статус - {task.Status}");

            Console.WriteLine("Введите stop для отмены операции или Enter чтобы продолжить");
            string s = Console.ReadLine();
            if (s == "stop")
            {
                cancelTokenSource.Cancel();
            }

            task.Wait();
            Console.WriteLine($"\nСтатус - {task.Status}");

            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed; //возвращает общее затраченное время
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            Console.WriteLine("RunTime " + elapsedTime);
            //16.3
            Task<int>[] taskArray = new Task<int>[3] {
                new Task<int>(() => (int)Math.PI + (int)Math.E),
                new Task<int>(() => (int)Math.PI * (int)Math.E),
                new Task<int>(() => (int)Math.PI / (int)Math.E)
            };

            foreach (Task t in taskArray)
                t.Start();

            Task<int> taskArray2 = new Task<int>(() => (taskArray[0].Result + taskArray[0].Result + taskArray[0].Result) / 2);
            taskArray2.Start();
            Console.WriteLine($"Результат выражения: {taskArray2.Result}");
            //16.4a
            Task task1 = new Task(() => Console.WriteLine($"\nИдентификатор - {Task.CurrentId}"));
            Task task2 = task1.ContinueWith((Task t) => {
                Console.WriteLine($"\nИдентификатор - {Task.CurrentId}");
                Console.WriteLine($"Идентификатор прошлой задачи - {t.Id}");
            });
            Task task3 = task2.ContinueWith((Task t) => {
                Console.WriteLine($"\nИдентификатор - {Task.CurrentId}");
                Console.WriteLine($"Идентификатор прошлой задачи - {t.Id}");
            });
            task1.Start();
            task3.Wait();
            //16.4б
            Task<int> what = Task.Run(() => Enumerable.Range(1, 10).Count(n => n % 2 == 0));
            // получаем объект продолжения
            var awaiter = what.GetAwaiter();
            // что делать после окончания предшественника
            awaiter.OnCompleted(() =>
            {
                int res = awaiter.GetResult();
                Console.WriteLine("Нечетных чисел: " + res);
            });
            //16.5
            Random r = new Random();
            int million = 1000000;
            int[,] result = new int[11, million];
            int[] result1 = new int[million];

            Stopwatch stopWatch1 = new Stopwatch();
            stopWatch1.Start();
  
            Parallel.For(1, million, (x) => {  
                result[0, x] = r.Next();
                result[2, x] = r.Next();
                result[4, x] = r.Next();
            });

            stopWatch1.Stop();
            TimeSpan ts1 = stopWatch1.Elapsed; //возвращает общее затраченное время
            string elapsedTime1 = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts1.Hours, ts1.Minutes, ts1.Seconds, ts1.Milliseconds);
            Console.WriteLine("Время выполнения распараллеленого цикла For " + elapsedTime1);

            Stopwatch stopWatch2 = new Stopwatch();
            stopWatch2.Start();

            for (int i = 1; i < million; i++)
            {
                result[1, i] = r.Next();
                result[3, i] = r.Next();
                result1[i] = r.Next();
            }

            stopWatch2.Stop();
            TimeSpan ts2 = stopWatch2.Elapsed; //возвращает общее затраченное время
            string elapsedTime2 = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts2.Hours, ts2.Minutes, ts2.Seconds, ts2.Milliseconds);
            Console.WriteLine("Время выполнения цикла for " + elapsedTime2);

            Stopwatch stopWatch3 = new Stopwatch();
            stopWatch3.Start();

            foreach (var e in result1)
            {
                
            }

            stopWatch3.Stop();
            TimeSpan ts3 = stopWatch3.Elapsed; //возвращает общее затраченное время
            string elapsedTime3 = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts3.Hours, ts3.Minutes, ts3.Seconds, ts3.Milliseconds);
            Console.WriteLine("Время выполнения цикла foreach " + elapsedTime3);

            Stopwatch stopWatch4 = new Stopwatch();
            stopWatch4.Start();

            Parallel.ForEach(result1, (x) => {

            });

            stopWatch4.Stop();
            TimeSpan ts4 = stopWatch4.Elapsed; //возвращает общее затраченное время
            string elapsedTime4 = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts4.Hours, ts4.Minutes, ts4.Seconds, ts4.Milliseconds);
            Console.WriteLine("Время выполнения распараллеленого цикла ForEach " + elapsedTime4);
            //16.6
            Console.WriteLine("\n");
            Parallel.Invoke(new Action[] {
                new Action(() => Console.WriteLine("1")),
                new Action(() => Console.WriteLine("2")),
                new Action(() => Console.WriteLine("3")),
                new Action(() => Console.WriteLine("4")),
                new Action(() => Console.WriteLine("5"))
            });
            //16.7
            OnChange = () =>
            {
                foreach (var i in blockingCollection)
                {
                    Console.WriteLine("Количество продукции: " + i);
                }
            };

            blockingCollection = new BlockingCollection<int>(5);
            CancellationTokenSource Cancel = new CancellationTokenSource();
            Task[] pr = new Task[5];
            Task[] Cons = new Task[10];
            for (int i = 0; i < 5; i++)
            {
                pr[i] = new Task(Producer, Cancel.Token);
                pr[i].Start();
            }
            for (int i = 0; i < 10; i++)
            {
                Cons[i] = new Task(Consumer, Cancel.Token);
                Cons[i].Start();
            }

            Thread.Sleep(5000);

            // вырубаем шарманку
            Cancel.Cancel();
            Working = false;
            Thread.Sleep(3000);
            //16.8
            FactorialAsync();
            Console.ReadLine();
        }
        static bool Working = true;

        public static void Producer()
        {
            if (!Working) return;
            mod++;
            for (int i = 3 * (mod); i < 4 * mod; i++)
            {
                Thread.Sleep(100 * mod);
                blockingCollection.Add(i);
                Console.WriteLine("Добавлен продукт: " + i);
                OnChange();
            }
        }

        public static void Consumer()
        {
            mod++;
            int i;
            while (!blockingCollection.IsCompleted)
            {
                Thread.Sleep(75 * mod / 2);
                if (blockingCollection.TryTake(out i))
                {
                    Console.WriteLine("Продан продукт: " + i);
                    OnChange();
                }
                else
                {
                    Console.WriteLine("Пусто!");
                }
                if (!Working) break;
            }
        }
        static BlockingCollection<int> blockingCollection = new BlockingCollection<int>(5);

        delegate void ChangeCollection();

        static event ChangeCollection OnChange;

        static int mod = 1;

        static void Factorial()
        {
            int result = 1;
            for (int i = 1; i <= 6; i++)
            {
                result *= i;
            }
            Console.WriteLine($"Факториал равен {result}");
        }

        // определение асинхронного метода
        static async void FactorialAsync()
        {
            Console.WriteLine("Начало метода FactorialAsync"); // выполняется синхронно
            await Task.Run(() => Factorial());                // выполняется асинхронно
            Console.WriteLine("Конец метода FactorialAsync");
        }
    }
}
