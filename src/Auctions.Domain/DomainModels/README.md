# Domain models

Note that in the domain models folder we have the data and business logic of an application.

The auction domain is implementing two different auction models. Typically this is not how it is done in business code. You would instead choose one auction type (the most common one being timed ascending auction, sometimes also referred to as English auction).

For many auction implementations (and in some IRL auctions) you have hidden sellers and bidders.

The intent of the domain core logic is to be as free as possible from any external dependencies. This is reminiscent of the concept [pure functions](https://en.wikipedia.org/wiki/Pure_function) but instead called [clean](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) as you have objects that can in their invocation can be seen as pure if consider the pattern:

```csharp
public CleanClass
{
    private readonly IRepository _repository;
    public CleanClass(IRepository repository){...}
    public async Task ExecuteBusinessLogic()
    {
        var state = await repository.GetCurrentState();
        if (state.Condition)
        {
            await _dependency.DoTheThing();
        }
    }
}
public class Program
{
    public static async Task Main()
    {
        //...
        var cleanClass = di.GetRequiredService<CleanClass>();
        await cleanClass.ExecuteBusinessLogic();
    }
}
```

can be rewritten into:

```csharp
public static PureModule
{
    public static IEnumerable<SideEffect> ExecuteBusinessLogic(CurrentState state)
    {
        if (state.Condition)
        {
            yield return new DoTheThing();
        }
    }
}

public class Program
{
    public static async Task Main()
    {
        //...
        var repo = di.GetRequiredService<IRepository>();
        foreach (var sideEffect in PureModule.ExecuteBusinessLogic(await repo.GetCurrentState()))
        {
            // interpret the side effects:
            switch (sideEffect)
            {
                case DoTheThing:
                    await repo.DoTheThing();
                    break;
                /// ...
            }
        }
    }
}
```

Note that the clean class has a [color to the method](https://journal.stuffwithstuff.com/2015/02/01/what-color-is-your-function/), but can be tested quite easily by using a mock object and has some similarity to a pure function.

Most programmers would argue to use the CleanClass rather than a pure function in this case since the common idioms are to let [Tasks](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl) and impure invocations bleed into the code.

Since we have a simple domain we have instead opted for the approach of having the infrastructure execute methods on the domain model objects in order to be a bit more object oriented in the sense that the domain objects actually receive the commands and interpret them. A natural step would be to allow `Task<>` as a return type of command handler (but that can be left as an exercise to the reader).

```csharp
public abstract class Auction : IEntity
{
    //...
    public IResult<Bid,Errors> TryAddBid(CreateBidCommand model, IUserContext userContext, ITime time)
    {
       //...
    }
}
```

Note that we do not try to make the try add bid method be pure (IUserContext and ISystemClock are both impure),but we do make it easy to test.
