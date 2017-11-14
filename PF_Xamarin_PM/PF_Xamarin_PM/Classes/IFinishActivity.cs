using System;
using System.Collections.Generic;
using System.Text;

namespace PF_Xamarin_PM
{

    public enum ReturnResult
    {
        Successful,
        Failed,
        UnCompleted
    }

    /// <summary>
    /// El tipo de dato generico que retorna una pagina en el evento return
    /// </summary>
    /// <typeparam name="T">El tipo de la data que retorna</typeparam>
    public struct ReturnInfo<T>
    {
        public ReturnResult Result { get; private set; }
        public T Data { get; private set; }

        public ReturnInfo(ReturnResult result, T data) : this()
        {
            Result = result;
            Data = data;
        }

        public override string ToString()
        {
            return string.Format("Result: {0} with return Data: {1}", Result.ToString(), Data.ToString());
        }
    }

    /// <summary>
    /// Interfaz para manejar el evento de finalizar una actividad
    /// y el retorno de datos
    /// </summary>
    /// <typeparam name="T">El tipo de dato de la return data</typeparam>
    public interface IFinishActivity<T>
    {
        event EventHandler<ReturnInfo<T>> FinishActivity;
    }
}
