using System.ComponentModel.DataAnnotations;

namespace BlogProjectDotNET_9.Models.ViewModels
{

        public class CategoryViewModel
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public int PostCount { get; set; }
        }
    }