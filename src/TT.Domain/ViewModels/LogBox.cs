namespace tfgame.ViewModels
{
    public class LogBox
    {
        public string ResultMessage { get; set; }
        public string AttackerLog { get; set; }
        public string VictimLog { get; set; }
        public string LocationLog { get; set; }

        public void Add(LogBox input)
        {
            ResultMessage += input.ResultMessage;
            AttackerLog += input.AttackerLog;
            VictimLog += input.VictimLog;
            LocationLog += input.LocationLog;
        }

    }

    public class ErrorBox
    {
        public bool HasError { get; set; }
        public string Error { get; set; }
        public string SubError { get; set; }
    }

    //public class ViewModel
    //{

    //}

    //public class ClassA
    //{

    //}

    //public class ClassB
    //{

    //}

    //public class ClassADetails
    //{

    //}

    //public class ClassADetails
    //{

    //}




}