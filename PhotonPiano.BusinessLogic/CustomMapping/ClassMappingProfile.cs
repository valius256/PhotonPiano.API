
using Mapster;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.BusinessModel.Tution;
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

            config.NewConfig<Tuition, TuitionWithStudentClassModel>()
                .Map(dest => dest.StudentClass.StudentFullName, src => src.StudentClass.Student.FullName);

            config.NewConfig<SystemConfigsModel, SystemConfig>()
                .Map(dest => dest.ConfigValue, String.Empty);

            // config.NewConfig<Slot, SlotModel>()
            //     .Map(dest => dest.Teacher, src => src.Teacher.FullName ?? src.Teacher.UserName)
            //     ;
            //
            
        }
    }
}
