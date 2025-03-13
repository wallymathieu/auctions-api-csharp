# Commands

Typically commands refers to a pattern used to encapsulate and represent user actions or requests that modify data or
trigger specific operations in an application. Note that instead of implementing the domain logic in the command as
mainly described in
the [Design Patterns: Elements of Reusable Object-Oriented Software](https://en.wikipedia.org/wiki/Design_Patterns) on
command pattern, the pattern used is more in line with what is described
related to [Unidraw](https://dl.acm.org/doi/10.1145/98188.98197) (also described in Design Patterns ...), decoupling
interpretation of the command from the command class.

From what I have seen the modern usage of the pattern usually decouples the data structure of the
command from the function implementation (usually called a command handler or request handler). Using a separate class
helps when the command class is instantiated
by [Spring data binding](https://www.baeldung.com/spring-mvc-custom-data-binder)
or [Asp.Net Core model binding](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-7.0),
since you then would not be able to use dependency injection through the constructor of the command.

Note that instead of having business logic separated from the domain model in a domain script such as a service, we have
the method on the domain classes directly. This is in order to avoid
the [anemic domain model](https://martinfowler.com/bliki/AnemicDomainModel.html) anti-pattern. The domain model (both in
the DDD sense and [P of EAA sense](https://martinfowler.com/eaaCatalog/domainModel.html)) should be responsible for its
own behavior. It would be fine to use a domain script (and I do use this patterns for code that is
more [data oriented](https://www.manning.com/books/data-oriented-programming)), but this code base is intended to be
written in a more object oriented style.