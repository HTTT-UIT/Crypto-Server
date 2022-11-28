using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Common.Commands
{
    public abstract class BaseCommand
    {
        [FromHeader(Name = "account-id")]
        public Guid ProfileId { get; set; }
    }
}