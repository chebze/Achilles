# Habbo Messaging System Documentation

## Overview

The Achilles Habbo messaging system provides a robust framework for handling communication between the client and server. The system is built around two main concepts: **Incoming Messages** (client-to-server) and **Outgoing Messages** (server-to-client), with automatic message type resolution and content parsing.

## Architecture

### Core Components

1. **Message Headers** - Unique numeric identifiers for message types
2. **Message Type Resolver** - Automatic registration and instantiation of message handlers
3. **Content Parsing** - Structured reading of encoded message data
4. **Context Management** - Dependency injection and request scoping

### Directory Structure

```
src/Habbo/Messaging/
├── Abstractions/
│   ├── MessageHeader.cs              # Message header wrapper
│   ├── Incoming/
│   │   ├── IncomingMessage.cs        # Base class for incoming messages
│   │   ├── IncomingMessageContent.cs # Content parsing utilities
│   │   └── IncomingMessageContext.cs # Request context with services
│   └── Outgoing/
│       └── OutgoingMessage.cs        # Base class for outgoing messages
├── Incoming/                         # Client-to-server message implementations
│   ├── Handshake/
│   ├── User/
│   ├── Navigator/
│   └── ... (organized by feature)
├── Outgoing/                         # Server-to-client message implementations
│   ├── Handshake/
│   ├── User/
│   ├── Alert/
│   └── ... (organized by feature)
├── IncomingMessageAttribute.cs       # Attribute for marking message types
└── IncomingMessageTypeResolver.cs    # Automatic message type registration
```

## Incoming Messages

### Basic Structure

Incoming messages represent client-to-server communication and must inherit from the `IncomingMessage` base class.

#### Key Components:

1. **Message Attribute** - Associates a numeric header with the message type
2. **Content Parsing** - Extracts structured data from the raw message
3. **Handling Logic** - Processes the message and optionally responds
4. **Context Access** - Access to services, database, user state, etc.

### Implementation Pattern

```csharp
[IncomingMessage(headerNumber)]
public class ExampleMessage : IncomingMessage
{
    // Properties for parsed data
    public string SomeProperty { get; private set; }
    public int AnotherProperty { get; private set; }

    // Constructor (required)
    public ExampleMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    // Parse the message content (required)
    protected override void Parse(IncomingMessageContent content)
    {
        this.SomeProperty = content.ReadString();
        this.AnotherProperty = content.ReadWiredInt();
    }

    // Optional: Synchronous handling
    public override void Handle(IncomingMessageContext ctx)
    {
        // Handle the message synchronously
    }

    // Optional: Asynchronous handling
    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        // Handle the message asynchronously
        await SomeAsyncOperation();
    }

    // Optional: Synchronous response
    public override OutgoingMessage? Respond(IncomingMessageContext ctx)
    {
        return new SomeResponseMessage();
    }

    // Optional: Asynchronous response
    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        var result = await SomeAsyncOperation();
        return new SomeResponseMessage(result);
    }
}
```

### Content Parsing Methods

The `IncomingMessageContent` class provides various methods for reading structured data:

#### Integer Reading
- `ReadWiredInt()` - Reads VL64 encoded integer
- `ReadBase64Int()` - Reads 2-character Base64 encoded integer
- `ReadBase64Int(int length)` - Reads Base64 integer of specific length

#### Boolean Reading
- `ReadWiredBoolean()` - Reads VL64 encoded boolean (1 = true, 0 = false)
- `ReadBase64Boolean()` - Reads single character Base64 boolean
- `ReadIntegerBoolean()` - Reads character-based boolean ('1' or '0')

#### String Reading
- `ReadString()` - Reads length-prefixed string (2-byte Base64 length + content)
- `ReadRemainingString()` - Reads all remaining content as string
- `ReadUntil(string delimiter)` - Reads until specific delimiter
- `ReadUntil(char delimiter)` - Reads until specific character
- `ReadLength(int length)` - Reads fixed-length string

#### Navigation and Utility
- `ReadChar()` - Reads single character
- `Peek()` - Peeks at next character without advancing
- `Skip(int count)` - Skips specified number of characters
- `SkipString(string value)` - Skips specific string if present

#### Properties
- `Content` - The raw message content
- `Index` - Current reading position
- `Length` - Total content length
- `Remaining` - Remaining unread characters

### Message Context

The `IncomingMessageContext` provides access to:

- **Services** - Dependency injection container
- **Connection** - Current TCP connection
- **Database** - Entity Framework database context
- **Configuration** - Habbo configuration settings
- **Logger** - Connection-specific logger
- **User** - Currently authenticated user (if any)
- **Rank** - User's rank configuration
- **Room** - Current room (if user is in a room)
- **ConnectionManager** - Access to all active connections

### Example Implementation

```csharp
[IncomingMessage(4)]
public class TryLoginMessage : IncomingMessage
{
    public string Username { get; private set; }
    public string Password { get; private set; }

    public TryLoginMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        this.Username = content.ReadString();
        this.Password = content.ReadString();
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        var user = await ctx.Database.Users.FirstOrDefaultAsync(u =>
            u.Username == this.Username &&
            u.Password == this.Password
        );

        if (user is null)
            return new LocalizedErrorMessage("Login incorrect");

        // Handle successful login...
        return new LoginMessage();
    }
}
```

## Outgoing Messages

### Basic Structure

Outgoing messages represent server-to-client communication and inherit from the `OutgoingMessage` base class.

#### Key Features:

1. **Fluent API** - Chain method calls for building message content
2. **Multiple Constructors** - Support various initialization patterns
3. **Automatic Conversion** - Implicit conversion to `Message` objects
4. **Flexible Content Building** - Support for different data types and encodings

### Implementation Pattern

```csharp
public class ExampleOutgoingMessage : OutgoingMessage
{
    // Constructor with header only
    public ExampleOutgoingMessage() : base(headerNumber)
    {
        // Build message content using fluent API
        this.AppendString("Hello")
            .AppendWired(42)
            .AppendBoolean(true);
    }

    // Constructor with parameters
    public ExampleOutgoingMessage(string message, int value) : base(headerNumber)
    {
        this.Build(message, value);
    }

    // Constructor with context (for context-dependent data)
    public ExampleOutgoingMessage(IncomingMessageContext ctx) : base(headerNumber)
    {
        var user = ctx.User;
        if (user != null)
        {
            this.AppendString(user.Username)
                .AppendWired(user.Id);
        }
    }

    // Private helper method for complex building logic
    private void Build(string message, int value)
    {
        this.AppendString(message)
            .AppendWired(value)
            .AppendChar(1); // Special character
    }
}
```

### Content Building Methods

The `OutgoingMessage` class provides various methods for building message content:

#### Data Appending
- `AppendBoolean(bool value)` - Appends VL64 encoded boolean
- `AppendWired(object o)` - Appends VL64 encoded integer
- `AppendBase64(object o)` - Appends Base64 encoded integer
- `Append(string s)` - Appends raw string
- `Append(object o)` - Appends object's string representation

#### String Appending
- `AppendString(object o)` - Appends string with ASCII 2 terminator
- `AppendString(object o, int breaker)` - Appends string with custom terminator
- `AppendString(object o, char breaker)` - Appends string with character terminator

#### Character Appending
- `AppendChar(int c)` - Appends character by ASCII code

#### Message Conversion
- `ToMessage()` - Converts to `Message` object with proper formatting
- Implicit conversion to `Message` is also supported

### Example Implementation

```csharp
public class SendUserInfoMessage : OutgoingMessage
{
    public SendUserInfoMessage(IncomingMessageContext ctx) : base(5)
    {
        var user = ctx.User;
        if(user is null)
            return;

        this.Build(
            user.Id,
            user.Username,
            user.Figure,
            user.Gender.ToString(),
            user.Motto,
            user.Tickets,
            user.PoolFigure,
            user.Film,
            false
        );
    }

    public SendUserInfoMessage(int userId, string username, string figure, string gender, string motto, int tickets, string poolFigure, int film, bool isReceiveNews) : base(5)
    {
        this.Build(userId, username, figure, gender, motto, tickets, poolFigure, film, isReceiveNews);
    }

    private void Build(
        int userId,
        string username,
        string figure,
        string gender,
        string motto,
        int tickets,
        string poolFigure,
        int film,
        bool isReceiveNews
    )
    {
        this.AppendString(userId);
        this.AppendString(username);
        this.AppendString(figure);
        this.AppendString(gender);
        this.AppendString(motto);
        this.AppendWired(tickets);
        this.AppendString(poolFigure);
        this.AppendWired(film);
        this.AppendBoolean(isReceiveNews);
    }
}
```

## Message Type Resolution

### Automatic Registration

The `MessageTypeResolver` class automatically discovers and registers all incoming message types using reflection:

1. **Assembly Scanning** - Scans the executing assembly for types
2. **Type Filtering** - Finds classes that inherit from `IncomingMessage`
3. **Attribute Processing** - Extracts header values from `IncomingMessageAttribute`
4. **Registration** - Maps header IDs to message types in a dictionary

### Runtime Resolution

When a message is received:

1. **Length Parsing** - Reads 3-character Base64 encoded packet length
2. **Header Extraction** - Reads 2-character Base64 encoded header
3. **Type Lookup** - Finds registered message type for the header
4. **Instantiation** - Creates message instance with header and content
5. **Content Parsing** - Automatically calls `Parse()` method

### Manual Override

The resolver supports runtime type replacement:

```csharp
// Replace with specific type
MessageTypeResolver.ReplaceResolver(4, typeof(CustomLoginMessage));

// Replace with generic method
MessageTypeResolver.ReplaceResolver<CustomLoginMessage>(4);
```

### Error Handling

- **Unknown Headers** - Collects exceptions for unknown message types
- **Multiple Messages** - Processes multiple messages in single packet
- **Aggregate Exceptions** - Throws `AggregateException` with all parsing errors

## Message Headers

### Header Structure

Message headers are managed by the `MessageHeader` class:

- **Value** - Integer header value
- **Encoded** - Base64 encoded string representation
- **Implicit Conversions** - Automatic conversion between int, string, and MessageHeader

### Usage Examples

```csharp
// Different ways to create headers
MessageHeader header1 = 123;                    // From int
MessageHeader header2 = "abc";                  // From encoded string
MessageHeader header3 = new MessageHeader(123); // Explicit constructor

// Automatic conversions
int value = header1;        // To int
string encoded = header1;   // To encoded string
```

## Encoding System

### Base64 Encoding

Used for headers and length prefixes:
- 2-character encoding for headers
- 3-character encoding for packet lengths
- Custom Habbo Base64 character set

### VL64 Encoding (Variable Length)

Used for most integer data:
- Efficient encoding for various integer sizes
- Used by `AppendWired()` and `ReadWiredInt()` methods
- Supports both positive and negative values

## Best Practices

### Incoming Messages

1. **Use Clear Naming** - Message names should describe their purpose
2. **Validate Input** - Always validate parsed data before processing
3. **Handle Errors Gracefully** - Use try-catch blocks for database operations
4. **Prefer Async Methods** - Use `HandleAsync` and `RespondAsync` for I/O operations
5. **Keep Parse Simple** - Only extract data in `Parse()`, handle logic elsewhere
6. **Use Context Services** - Leverage dependency injection through context

### Outgoing Messages

1. **Multiple Constructors** - Provide flexible initialization options
2. **Context Constructor** - Offer context-based constructor when applicable
3. **Helper Methods** - Use private methods for complex building logic
4. **Fluent Chaining** - Chain append methods for readability
5. **Type Safety** - Validate parameters before building content

### Message Organization

1. **Feature Grouping** - Organize messages by feature area (User, Room, etc.)
2. **Consistent Headers** - Use logical header numbering scheme
3. **Documentation** - Comment complex parsing or building logic
4. **Testing** - Write unit tests for message parsing and building

### Error Handling

1. **Graceful Degradation** - Handle missing or invalid data gracefully
2. **User Feedback** - Send appropriate error messages to clients
3. **Logging** - Log important events and errors with context
4. **Connection Safety** - Handle connection drops and timeouts

## Common Patterns

### Authentication Check

```csharp
public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
{
    if (ctx.User is null)
        return new ErrorMessage("Not authenticated");
    
    // Continue with authenticated logic...
}
```

### Database Operations

```csharp
public override async Task HandleAsync(IncomingMessageContext ctx)
{
    try
    {
        var entity = await ctx.Database.SomeEntities
            .FirstOrDefaultAsync(e => e.Id == someId);
        
        if (entity != null)
        {
            entity.UpdateProperty();
            await ctx.Database.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        ctx.Logger.LogError(ex, "Failed to update entity");
    }
}
```

### Broadcasting to Multiple Connections

```csharp
public override async Task HandleAsync(IncomingMessageContext ctx)
{
    var message = new SomeNotificationMessage();
    
    var connections = ctx.ConnectionManager.Connections
        .Where(c => c.Metadata.OfType<User>().Any())
        .ToList();
    
    foreach (var connection in connections)
    {
        await connection.SendMessageAsync(message);
    }
}
```

This messaging system provides a robust, extensible framework for handling all client-server communication in the Habbo server, with automatic type resolution, structured content parsing, and comprehensive context management. 