﻿namespace llmChat.Helpers
{
    public class CharacterQuery
    {
        public string? Name { get; set; } = null;

        public string? SortBy { get; set; } = null;

        public bool IsDescending { get; set; } = false;
    }
}
