using System.ComponentModel.DataAnnotations.Schema;

namespace RestApiCRUD.Models
{
    public class Quest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int QuestId { get; set; }
        public string Description { get; set; }
        public int Reward { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
