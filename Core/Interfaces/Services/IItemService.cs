﻿using Core.Entities;

namespace Core.Interfaces.Services;

public interface IItemService<T> : IService<T> where T : class
{
    Task AddToCharacterAsync(Character character, T item);
    Task RemoveFromCharacterAsync(Character character, T item);
}
