# Domain

The core of auctions is intended to be the meat of the application: the business domain.

## Use cases

The mutating use cases in this domain are:

- Create an auction
- Place a bid on an auction

Since these use cases do not need additional external dependencies, they are implemented as methods on the domain entity
itself.

## Domain entities

The domain entities are:

- Auction : the auction itself. Note that the auction is the aggregate root of the domain. This means that all
  operations on the domain entities are done through the auction. Note also that the auction has two sub classes: "
  single sealed bid" and "timed ascending" auctions. These are the two types of auctions that are supported by the
  application.
- Bid : a bid on an auction
- User : a user of the application

