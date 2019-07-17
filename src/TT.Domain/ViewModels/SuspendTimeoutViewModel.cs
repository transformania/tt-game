using System;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
   public class SuspendTimeoutViewModel
   {
       public Player Player { get; set; }
       public string UserId { get; set; }
       public DateTime date { get; set; }
   }
}
