using System;
using System.Collections.Generic;


public class Ticket
{
    public TicketSource Source { get; }
    public TicketKey Id { get; }
    public OrgTodoState OrgState { get; }
    public string Summary { get; }
    public List<string> Labels { get; } = new();
    public Assignee AssignedTo { get; }
    public Reporter Reporter { get; }
    public TicketType Type { get; }
    public Priority Priority { get; }
    public TicketStatus JiraStatus { get; }
    public DateOnly? Scheduled { get; }
    public DateOnly? Deadline { get; }
    public Uri JiraUrl { get; }
    public string Description { get; }

    public Ticket(
        TicketKey id,
        OrgTodoState orgState,
        string summary,
        IEnumerable<string> labels,
        Assignee assignedTo,
        Reporter reporter,
        TicketType type,
        Priority priority,
        TicketStatus jiraStatus,
        DateOnly? scheduled,
        DateOnly? deadline,
        Uri jiraUrl,
        string description) : this(TicketSource.Jira, id, orgState, summary, labels, assignedTo, reporter, type, priority, jiraStatus, scheduled, deadline, jiraUrl, description)
    {
        // This constructor is for cases where the source is always Jira.
    }

    public Ticket(TicketSource source
        , TicketKey id
        , OrgTodoState orgState
        , string summary
        , IEnumerable<string> labels
        , Assignee assignedTo
        , Reporter reporter
        , TicketType type
        , Priority priority
        , TicketStatus jiraStatus
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
