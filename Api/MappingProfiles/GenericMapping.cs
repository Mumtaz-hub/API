using System.Collections.Generic;
using AutoMapper;
using ViewModel;


namespace Api.MappingProfiles
{
    public class GenericMapping : Profile
    {
        public GenericMapping()
        {
            CreateMap<KeyValuePair<int, string>, LookUpViewModel>();
            CreateMap<KeyValuePair<byte, string>, LookUpViewModel>();
        }
    }
}
