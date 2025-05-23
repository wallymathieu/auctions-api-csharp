# Infrastructure

This project contains the infrastructure code for the application.

## Main structure

### Database

We have an implicit dependency on entity framework core since the classes are written in a way that is compatible with
EF core but might not be compatible with any other framework.

I would have preferred to use some other migration framework, but EF migrations can be OK enough for a single developer
database schema.

### Cache

Cache logic is generally not a core business concern but a operations requirement in order for the application to
perform well. It is seen as a cross cutting concern and is therefore placed in the infrastructure part.

### Services

Services in this application does not implement any business logic but glues together the application domain with the
database. They are therefore placed in the infrastructure part. We could implement the services through the means of
more advanced patterns but that makes it harder for developers to get into the infrastructure (see the branch with that
implementation as reference).

### Logging

We do not replicate a more complete solution since this is not a real application. In a real application you would
expect the developers to add different types of logging in order to simplify operating the software. Typically you could
have some mean to log both business events, then use ILogger to log exceptions and trace information (these things
should be seen as separate concerns since they are used for very different purposes and have different persistence
lifecycle).

## Comparison with Vertical Slice Architecture

The [vertical slice architecture](https://web.archive.org/web/20230328220230/https://jimmybogard.com/vertical-slice-architecture/)
is a reaction
against [clean architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html).
Some of the issue could be due to a formalization of the onion/hexagonal/clean architecture without questioning the
reason for separating infrastructure and and external dependencies from business code. Look at the context of when the
article/book was written. He also tries to give you ample examples of how he has written existing systems (but that is
in the context of the popular frameworks and languages of that time). We need to learn of the spirit of the architecture
and not the letter. The letter is a snapshot of a specific time and place. The spirit is the reason for the
architecture. In that sense I do not think that the two architectures are mutually exclusive. Reading up on what [Ardalis writes](https://ardalis.com/stories-too-big-%E2%80%93-vertical-slices/) suggests that you could apply vertical slices to any layered approach.

In this case we have a small (but complicated) domain with features of the domain tied to the domain entities, we could
see
this entire app as a slice since it is intended to only model a single bounded context. We could have used transaction
scripts
and moved the code that now resides in the domain entities into what is now glue services.

## Limitations

### Concurrency

We are not dealing with what could happen if you send bids at the same time. How persistence is dealt with is overly simplistic. This could of course (as we often see) be the case even for production apps.

What happens when multiple bidders send the same bid at the same time? We implement optimistic concurrency using a [version token in Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=fluent-api). This means that if two users try bid on the same auction at the same time, one of them will succeed and the other will fail with a `DbUpdateConcurrencyException`.

In a real world case, you could have Terms of service with explicit limitations in order to limit the software complexity. You could also design user interface to not expect an immediate response (i.e. that you put the writes behind a queue).

### Administration

In a commercial site you would expect there to be back office staff such as support personal that can view information not accessible to regular users.
