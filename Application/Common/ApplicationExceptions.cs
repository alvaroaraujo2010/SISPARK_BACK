namespace sispark_api.Application.Common;

public sealed class ResourceNotFoundException(string message) : Exception(message);

public sealed class BusinessRuleException(string message) : Exception(message);

public sealed class ConflictResourceException(string message) : Exception(message);

public sealed class CredentialsRejectedException(string message) : Exception(message);
