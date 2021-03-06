﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using Swagger.Net.Annotations;

using Users.BLL.Base;
using Users.BLL.Models;
using Users.WebApi.Filters;

namespace Users.WebApi.Controllers
{
    [RoutePrefix("api/users")]
    [CustomExceptionFilter]
    public class UserController : ApiController
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a list of users.")]
        public async Task<IHttpActionResult> GetUsers()
        {
            var users = await _service.GetAllUsersAsync();

            return Ok(users);
        }

        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.Created, Description = "Returns a new user and it's location.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Invalid model state of a passed user.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "User with same LoginName already exists.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> CreateUser([FromBody]UserRequest createRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _service.CreateUserAsync(createRequest);
            var userLocation = $"api/users/{user.Id}";

            return Created(userLocation, user);
        }

        [HttpGet]
        [Route("{userId:int:min(1)}/addresses")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns list of addresses of user with passed id.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "User with passed id does not exist.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetUserAddresses([FromUri]int userId)
        {
            var addresses = await _service.GetUserAddressesAsync(userId);

            return Ok(addresses);
        }

        [HttpGet]
        [Route("{userId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns user with passed id.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "User with passed id does not exist.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetUser([FromUri]int userId)
        {
            var user = await _service.GetUserById(userId);

            return Ok(user);
        }

        [HttpGet]
        [Route("{userId:int:min(1)}/addresses/{addressId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns user's address.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "User with passed id does not exist or user don't have address with passed id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetAddress([FromUri]int userId, [FromUri]int addressId)
        {
            var address = await _service.GetUsersAddressByIdAsync(userId, addressId);

            return Ok(address);
        }

        [HttpPut]
        [Route("{userId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Updates user with passed id.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Invalid model state of a passed user.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "User with passed id does not exist.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "User with same LoginName already exists.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> UpdateUser([FromUri]int userId, [FromBody]UserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.UpdateUserAsync(userId, request);

            var response = Request.CreateResponse(HttpStatusCode.NoContent);
            return ResponseMessage(response);
        }

        [HttpPatch]
        [Route("{userId:int:min(1)}/lastName")]
        [SwaggerExample("lastName", "\"new last name\"")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Updates last name of the user with passed id.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Last name was null or empty.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "User with passed id does not exist.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> UpdateUserLastName([FromUri]int userId, [FromBody]string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
            {
                return BadRequest($"{nameof(lastName)} must not be null or empty.");
            }

            await _service.UpdateUserLastNameAsync(userId, lastName);

            var response = Request.CreateResponse(HttpStatusCode.NoContent);
            return ResponseMessage(response);
        }

        [HttpPatch]
        [Route("{userId:int:min(1)}/addresses/{addressId:int:min(1)}")]
        [SwaggerExample("lastName", "\"new last name\"")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Adds new address to the user with passed id.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Not valid address model state.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Address with same Description already exists.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "User with passed id does not exist or it does not have address with passed id.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> UpdateUserAddress([FromUri]int userId, [FromUri]int addressId, [FromBody]AddressRequest addressRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.UpdateUserAddressAsync(userId, addressId, addressRequest);

            var response = Request.CreateResponse(HttpStatusCode.NoContent);
            return ResponseMessage(response);
        }

        [HttpPost]
        [Route("{userId:int:min(1)}/addresses")]
        [SwaggerExample("lastName", "\"new last name\"")]
        [SwaggerResponse(HttpStatusCode.Created, Description = "Returns new address and it's location.")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Not valid address model state.")]
        [SwaggerResponse(HttpStatusCode.Conflict, Description = "Address with same Description already exists.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "User with passed id does not exist.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> AddNewAddress([FromUri]int userId, [FromBody]AddressRequest addressRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var address = await _service.AddNewAddressToUserAsync(userId, addressRequest);
            var addressLocation = $"api/users/{userId}/addresses/{address.Id}";

            return Created(addressLocation, address);
        }

        [HttpDelete]
        [Route("{userId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Deletes user with passed id.")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "User with passed id does not exist.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> DeleteUser([FromUri]int userId)
        {
            await _service.DeleteUserByIdAsync(userId);

            var response = Request.CreateResponse(HttpStatusCode.NoContent);
            return ResponseMessage(response);
        }
    }
}
