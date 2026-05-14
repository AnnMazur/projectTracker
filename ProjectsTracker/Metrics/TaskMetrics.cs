using System.Diagnostics.Metrics;

namespace ProjectsTracker.Metrics;

public class TaskMetrics
{
    private readonly Meter _meter;

    public Counter<int> SuccessfulTransitions { get; }

    public Counter<int> FailedTransitions { get; }

    public Counter<int> DuplicateEvents { get; }

    public Counter<int> Compensations { get; }

    public Histogram<double> TransitionDuration { get; }

    public TaskMetrics()
    {
        _meter = new Meter("ProjectsTracker.Workflow");

        SuccessfulTransitions =
            _meter.CreateCounter<int>("successful_transitions");

        FailedTransitions =
            _meter.CreateCounter<int>("failed_transitions");

        DuplicateEvents =
            _meter.CreateCounter<int>("duplicate_events");

        Compensations =
            _meter.CreateCounter<int>("compensations");

        TransitionDuration =
            _meter.CreateHistogram<double>("transition_duration_ms");
    }
}