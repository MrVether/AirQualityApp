using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirQualityApp.Models
{
    [Table("Country")]
    public class Country
    {
        [Key]
        [Required]
        [Column("Code")]
        public string Code { get; set; }

        [Column("Name")]
        public string Name { get; set; }
    }

    [Table("Coordinates")]
    public class Coordinates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Latitude")]
        public double Latitude { get; set; }

        [Column("Longitude")]
        public double Longitude { get; set; }
    }

    [Table("Date")]
    public class Date
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Utc")]
        public string Utc { get; set; }

        [Column("Local")]
        public string Local { get; set; }
    }

    [Table("Measurement")]
    public class Measurement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Location")]
        public string Location { get; set; }

        [Column("City")]
        public string? City { get; set; }

        [Column("Parameter")]
        public string Parameter { get; set; }

        [Column("Country")]
        public string Country { get; set; }

        [Column("Value")]
        public double Value { get; set; }

        [Column("Unit")]
        public string Unit { get; set; }

        [ForeignKey("DateId")]
        public Date Date { get; set; }

        [ForeignKey("CoordinatesId")]
        public Coordinates Coordinates { get; set; }

    }
}