
using Mapster;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.BusinessLogic.CustomMapping
{
    public class ClassMappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Class, ClassModel>()
               .Map(dest => dest.StudentNumber, src => src.StudentClasses.Count);
            
            //config.NewConfig<StudentClass, StudentClassModel>()
            //    .Map(dest => dest.C, src => src.Class.Name);
        }
    }
}
