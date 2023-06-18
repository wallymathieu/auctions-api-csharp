# Commands

Typically commands refers to a pattern used to encapsulate and represent user actions or requests that modify data or
trigger specific operations in an application. Note that instead of implementing the domain logic in the command as done
in [Design Patterns: Elements of Reusable Object-Oriented Software](https://en.wikipedia.org/wiki/Design_Patterns) on
command pattern in page 359-366. Instead the modern usage of the pattern usually decouples the data structure of the
command from the function implementation (usually called a command handler or request handler). This is more in line with what is described
related to [Unidraw](https://dl.acm.org/doi/10.1145/98188.98197) but instead of naming the execution of the command
`interpret` I've mostly seen the use of `handle` since the command might be instantiated by [Spring data binding](https://www.baeldung.com/spring-mvc-custom-data-binder) or [Asp.Net Core model binding](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-7.0).