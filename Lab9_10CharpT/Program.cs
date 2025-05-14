// See https://aka.ms/new-console-template for more information
using System;

namespace FacultyLifeEvent
{
    public delegate void FacultyEventHandler(object sender, FacultyEventArgs e);

    public class Faculty
    {
        string name;
        int days;
        public event FacultyEventHandler? FacultyDay;
        string[] reactions = Array.Empty<string>();
        private Random rnd = new Random();
        double eventProbability = 0.1;

        DeanOffice dean;
        StudentCouncil council;
        Teachers teachers;

        public Faculty(string name, int days)
        {
            this.name = name;
            this.days = days;

            // Підключаємо служби
            dean = new DeanOffice(this);
            council = new StudentCouncil(this);
            teachers = new Teachers(this);

            dean.On();
            council.On();
            teachers.On();
        }

        protected virtual void OnFacultyDay(FacultyEventArgs e)
        {
            Console.WriteLine($"\nУвага! День факультету {name}! День: {e.Day}");

            if (FacultyDay != null)
            {
                Delegate[] handlers = FacultyDay.GetInvocationList();
                reactions = new string[handlers.Length];
                int i = 0;
                foreach (FacultyEventHandler handler in handlers)
                {
                    handler(this, e);
                    reactions[i++] = e.Result;
                }
            }
        }

        public void SimulateLife()
        {
            bool eventHappened = false;
            for (int day = 1; day <= days; day++)
            {
                if (rnd.NextDouble() < eventProbability)
                {
                    FacultyEventArgs e = new FacultyEventArgs(day);
                    OnFacultyDay(e);
                    eventHappened = true;
                    foreach (string reaction in reactions)
                        Console.WriteLine(reaction);
                }
            }

            if (!eventHappened)
            {
                Console.WriteLine($"За {days} днів у факультету {name} не було жодного свята.");
            }
        }
    }

    public class FacultyEventArgs : EventArgs
    {
        public int Day { get; }
        public string Result { get; set; } = string.Empty;

        public FacultyEventArgs(int day)
        {
            Day = day;
        }
    }

    public abstract class Receiver
    {
        protected Faculty faculty;
        protected Random rnd = new Random();

        public Receiver(Faculty f)
        {
            faculty = f;
        }

        public void On()
        {
            faculty.FacultyDay += new FacultyEventHandler(React);
        }

        public void Off()
        {
            faculty.FacultyDay -= new FacultyEventHandler(React);
        }

        public abstract void React(object sender, FacultyEventArgs e);
    }

    public class DeanOffice : Receiver
    {
        public DeanOffice(Faculty f) : base(f) { }

        public override void React(object sender, FacultyEventArgs e)
        {
            e.Result = rnd.Next(10) > 5 ?
                "Деканат організував святкування!" :
                "Деканат нічого не організував.";
        }
    }

    public class StudentCouncil : Receiver
    {
        public StudentCouncil(Faculty f) : base(f) { }

        public override void React(object sender, FacultyEventArgs e)
        {
            e.Result = rnd.Next(10) > 3 ?
                "Студради провели концерт!" :
                "Студради нічого не підготували.";
        }
    }

    public class Teachers : Receiver
    {
        public Teachers(Faculty f) : base(f) { }

        public override void React(object sender, FacultyEventArgs e)
        {
            e.Result = rnd.Next(10) > 2 ?
                "Викладачі провели пари!" :
                "Викладачі відпочили.";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Faculty f = new Faculty("ІФТКН", 30);
            f.SimulateLife();
        }
    }
}

