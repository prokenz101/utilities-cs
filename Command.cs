namespace utilities_cs {
    abstract public class Command {
        abstract public string Run(string args);
        abstract public string Format(string args);
    }
}