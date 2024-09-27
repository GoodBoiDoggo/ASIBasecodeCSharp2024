using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Manager;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class SampleCrudService2 : ISampleCrudService2
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserDetailsRepository _userDetailsRepository;
        private readonly IUserPermissionRepository _userPermissionRepository;
        private readonly IMapper _mapper;

        public SampleCrudService2(IMapper mapper, IUserRepository userRepository, IUserDetailsRepository userDetailsRepository, IUserPermissionRepository userPermissionRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userDetailsRepository = userDetailsRepository;
            _userPermissionRepository = userPermissionRepository;
        }

        /// <summary>
        /// Retrieves all.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserViewModelEx> RetrieveAll(int? id = null, string firstName = null)
        {
            var userQuery = _userRepository.GetUsers().Where(x => x.Deleted != true);
            var userDetailsQuery = _userDetailsRepository.GetUserDetails();
            var userPermissionQuery = _userPermissionRepository.GetUserPermissions();

            var joinQuery = userQuery.Join(userDetailsQuery, user => user.UserId, userDetail => userDetail.UserId, (user, userDetail) => new { user, userDetail })
                            .GroupJoin(userPermissionQuery, usrDtl => usrDtl.user.UserId, usrPrmsn => usrPrmsn.UserId, (usrDtl, usrPrmsn) => new { usrDtl.user, usrDtl.userDetail, usrPrmsn })
                            .SelectMany(usrDtlPrmsn => usrDtlPrmsn.usrPrmsn.DefaultIfEmpty(), (usrDtlPrmsn, userPermission) => new { usrDtlPrmsn.user, usrDtlPrmsn.userDetail, userPermission });


            var data = joinQuery
                .Where(x => (!id.HasValue || x.user.UserId == id)
                        && (string.IsNullOrEmpty(firstName) || x.user.FirstName.Contains(firstName)))
                .Select(s => new UserViewModelEx
            {
                Id = s.user.UserId,
                Name = string.Concat(s.user.FirstName, " ", s.user.LastName),
                Detail1 = s.userDetail.Detail1 ?? "-",
                Detail2 = s.userDetail.Detail2 ?? "-",
                Permission1 = (s.userPermission.Permission1 ?? false) ? "OK" : "N/A",
                Permission2 = (s.userPermission.Permission2 ?? false) ? "OK" : "N/A",
                Description = s.user.Remarks,
            });
            return data;
        }

        public UserViewModel RetrieveUser(int id)
        {
            var data = _userRepository.GetUsers().FirstOrDefault(x => x.Deleted != true &&  x.UserId == id);
            var model = new UserViewModel
            {
                Id = data.UserId,
                UserCode = data.UserCode,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Password = PasswordManager.DecryptPassword(data.Password)
        };
            return model;
        }

        /// <summary>
        /// Adds the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void Add(UserViewModel model)
        {
            var newModel = new MUser();
            newModel.UserCode = model.UserCode;
            newModel.FirstName = model.FirstName;
            newModel.LastName = model.LastName;
            newModel.Password = PasswordManager.EncryptPassword(model.Password);
            newModel.UserRole = 1;

            _userRepository.AddUser(newModel);
        }

        /// <summary>
        /// Updates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void Update(UserViewModel model)
        {
            var existingData = _userRepository.GetUsers().Where(s => s.Deleted != true && s.UserId == model.Id).FirstOrDefault();
            existingData.UserCode = model.UserCode;
            existingData.FirstName = model.FirstName;
            existingData.LastName = model.LastName;
            existingData.Password = PasswordManager.EncryptPassword(model.Password);

            _userRepository.UpdateUser(existingData);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Delete(int id)
        {
            _userRepository.DeleteUser(id);
        }
    }
}
