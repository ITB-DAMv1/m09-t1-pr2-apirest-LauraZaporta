﻿using API.Model;

namespace API.DTOs
{
    public class GameGetDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TeamName { get; set; }
        public string? ImagePath { get; set; }
        public int? VoteCount { get; set; }
    }
}