# API Models

Instead of using the domain model directly in the API, we use separate DTO models in order
for the API not to be directly tied to the domain. We want to be able to serialize the domain
objects into cache using JSON, so using JsonIgnore will not help us in that case.

Using separate types helps maintain a clear separation between the API input and output and the
associated domain logic. Since we have separate API models we can easily version inside the Api project instead of having
to create a separate adapter service (such as a separate docker image) to do versioning logic.
Due to the separation you can also avoid accidentally exposing secret information such as bidder or seller
information not intended for all users of the API.

## AutoMapper

Read the [usage guide](https://www.jimmybogard.com/automapper-usage-guidelines/).