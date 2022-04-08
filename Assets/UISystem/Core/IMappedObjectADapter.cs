#if ILRuntime
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;

public class IMappedObjectAdapter : CrossBindingAdaptor
{
    static CrossBindingFunctionInfo<IMapper> Methed1 = new CrossBindingFunctionInfo<IMapper>("Mapper");
    static CrossBindingMethodInfo<IMapper> Methed2 = new CrossBindingMethodInfo<IMapper>("Initialize");
    public override Type BaseCLRType { 
        get 
        { 
            return typeof(IMappedObject);
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    public class Adaptor : IMappedObject, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        //必须要提供一个无参数的构造函数
        public Adaptor()
        {
            
        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } }


        IMapper IMappedObject.Mapper
        {
            get
            {
               return Methed1.Invoke(this.instance);
            }
        }

        public void Initialize(IMapper mapper)
        {
            Methed2.Invoke(this.instance,mapper);
        }
    }
}
#endif