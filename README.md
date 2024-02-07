# Orleans Explicit Streaming Issue Reproduction
## Overview
When a grain implicitly subscribes to its own events by applying [ImplicitStreamSubscription] attribute, other instances of that grain type are unable to explicitly subscribe to it and receive events.

## Repro
This code sample creates three grains with different GUID keys:

1. GrainA
2. OtherGrainA
3. GrainB

The GrainA type applies an Implicit subscription to notify it about its own events.

OtherGrainA and GrainB are instructed to subscribe to GrainA's event stream too. GrainB is included to demonstrate how a grain of a different type does *not* suffer from this issue.

Grain A is then told to publish a message on its stream and the consumers (3) write to the console when they receive this message.

### Expected Output:
All 3 consumers publish their console messages indicating they've received the message:

```
Grain B received message: hi
Grain A received its own message: hi
Other Grain A received message: hi
```

### Actual Output:
OtherGrainA never gets the stream message:

```
Grain A received its own message: hi
Grain B received message: hi
```

## Exploration
It seems like the static map of implicit subscriptions built up by Orleans is part of the problem. If you remove the [ImplicitStreamSubscription] from GrainA and re-run the sample OtherGrainA *does* see the message and the output appears as per the "expected" case above.

Suspect Orleans is verifying the explicit stream we setup against "existing" implicit streams in the static map and is determining that because the target grain type, ID and the namespace are the same that this explicit subscription *actually* refers to the implicit one GrainA has on itself, so it quietly drops the subscription on the floor. Perhaps?