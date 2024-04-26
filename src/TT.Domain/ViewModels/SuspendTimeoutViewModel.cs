using System;
using TT.Domain.Identity.Entities;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
   public class SuspendTimeoutViewModel
   {
       public User User { get; set; }
       public Player Player { get; set; } 
       public string UserId { get; set; }
       public int PlayerId { get; set; }
       public DateTime date { get; set; }
       public bool isPvPLocked { get; set; }
       public bool isAccountLocked { get; set; }
       public String lockoutMessage { get; set; }
   }
}
