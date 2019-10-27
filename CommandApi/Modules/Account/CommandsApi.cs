using System;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace CommandApi.Modules.Account
{
    [Route("/account")]
    [ApiController]
    public class CommandsApi
    {
        private readonly ApplicationService applicationService;

        public CommandsApi(ApplicationService appService)
        {
            applicationService = appService;
        }

        [HttpPost]
        public Task<IActionResult> Post(Commands.V1.CreateAccount command) => HandleOrThrow(command, app => applicationService.Handle(app));
        
        [Route("deposit")]
        [HttpPut]
        public Task<IActionResult> Put(Commands.V1.Deposit command) => HandleOrThrow(command, app => applicationService.Handle(app));

        [Route("withdraw")]
        [HttpPut]
        public Task<IActionResult> Put(Commands.V1.Withdraw command) => HandleOrThrow(command, app => applicationService.Handle(app));
               
        [Route("transfer")]
        [HttpPut]
        public Task<IActionResult> Put(Commands.V1.TransferAccount command) => HandleOrThrow(command, app => applicationService.Handle(app));

        [Route("close")]
        [HttpPut]
        public Task<IActionResult> Put(Commands.V1.CloseAccount command) => HandleOrThrow(command, app => applicationService.Handle(app));
       
        private static async Task<IActionResult> HandleOrThrow<T>(T request,Func<T,Task> handle)
        {
            try
            {
                await handle(request);
                return new OkResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);                
                return  new BadRequestResult();

            }
        }
    }
}
