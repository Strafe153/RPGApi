namespace Domain.Shared;

public record GetAllResult<T>(int Count, List<T> Items);