using Akka.Persistence.Query;

namespace Smeti.Projection;

public readonly record struct ChangeLastOffsetCommand(Offset Offset);