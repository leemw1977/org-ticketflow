public record OrgTodoState(string Value)
{
    public static OrgTodoState Todo => new("TODO");
    public static OrgTodoState InProgress => new("IN-PROGRESS");
    public static OrgTodoState Done => new("DONE");

    public override string ToString() => Value;
}
