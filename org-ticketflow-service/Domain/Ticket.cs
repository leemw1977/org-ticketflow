using System;
using System.Collections.Generic;


public class Ticket
{
    public TicketSource Source { get; }
    public Key Id { get; }
    public OrgTodoState OrgState { get; }
    public string Summary { get; }
    public List<string> Labels { get; } = new();
    public Assignee AssignedTo { get; }
    public Reporter Reporter { get; }
    public Type Type { get; }
    public Priority Priority { get; }
    public Status JiraStatus { get; }
    public DateOnly? Scheduled { get; }
    public DateOnly? Deadline { get; }
    public Uri JiraUrl { get; }
    public string Description { get; }

    public Ticket(
        Key id,
        OrgTodoState orgState,
        string summary,
        IEnumerable<string> labels,
        Assignee assignedTo,
        Reporter reporter,
        Type type,
        Priority priority,
        Status jiraStatus,
        DateOnly? scheduled,
        DateOnly? deadline,
        Uri jiraUrl,
        string description) : this(TicketSource.Jira, id, orgState, summary, labels, assignedTo, reporter, type, priority, jiraStatus, scheduled, deadline, jiraUrl, description)
    {
        // This constructor is for cases where the source is always Jira.
    }

    public Ticket(TicketSource source
        , Key id
        , OrgTodoState orgState
        , string summary
        , IEnumerable<string> labels
        , Assignee assignedTo
        , Reporter reporter
        , Type type
        , Priority priority
        , Status jiraStatus
        , DateOnly? scheduled
        , DateOnly? deadline
        , Uri jiraUrl
        , string description)
    {
        Source = source;
        Id = id;
        OrgState = orgState;
        Summary = summary;
        Labels.AddRange(labels);
        AssignedTo = assignedTo;
        Reporter = reporter;
        Type = type;
        Priority = priority;
        JiraStatus = jiraStatus;
        Scheduled = scheduled;
        Deadline = deadline;
        JiraUrl = jiraUrl;
        Description = description;
    }

    public bool IsOverdue() =>
        Deadline.HasValue && Deadline.Value < DateOnly.FromDateTime(DateTime.UtcNow.Date);
}
