using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskEmployee.Models
{
    public class Employee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string PayrollNumbers { get; set; }
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public int TelephoneNumber { get; set; }
        public int Mobile { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string PostCode { get; set; }
        public string Email { get; set; }
        public string StartDate { get; set; }

    }
}
