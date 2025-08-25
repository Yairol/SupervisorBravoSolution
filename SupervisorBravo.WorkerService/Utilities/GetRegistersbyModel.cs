namespace SupervisorBravo.WorkerService.Utilities
{
    /// <summary>
    /// Helpers para obtener los registros modbus según el modelo del dispositivo
    /// Por defecto viene el XR60CX
    /// </summary>
    public static class GetRegistersbyModel
    {
        public static ushort GetXRSetPointRegisterByModel(string modelName)
        {
            switch (modelName)
            {
                case "XR100D": return (ushort)876;

                default: return (ushort)863;

            }

        }
        public static ushort GetXRThawingRegisterByModel(string modelName)
        {
            switch (modelName)
            {

                case "XR100D": return (ushort)1280; //4369 Thawing Off ----- 4883 ON

                default: return (ushort)513;
            }

        }

        public static ushort GetXRCoolingRegisterByModel(string modelName)
        {
            switch (modelName)
            {
                case "XR100D": return (ushort)2049; //257 Cool ON ----- 0 OFF

                default: return (ushort)15;
            }
        }

        public static ushort GetXRFanRegisterByModel(string modelName)
        {
            switch (modelName)
            {
                case "XR100D": return (ushort)2048; //8224 FAN ON ----- 514 OFF

                default: return (ushort)15;

            }
        }
        public static ushort GetXROnOffRegisterByModel(string modelName)
        {
            switch (modelName)
            {
                case "XR100D": return (ushort)1030; //6501 ON ----- 6245 OFF

                default: return (ushort)512;

            }
        }
        /// <summary>
        /// Para detectar si los registros con logica bool son de tipo holding o de tipo coil
        /// True --- Tipo Holding
        /// False --- Tipo Coil
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static bool GetRegisterTypeByModel(string modelName)
        {
            switch (modelName)
            {
                case "XR100D": return true; //tipo holding

                default: return false; //tipo coil
            }
        }
        public static ushort GetXROnOffWriteRegisterByModel(string modelName)
        {
            switch (modelName)
            {
                case "XR100D": return (ushort)1280; //4883 ON ---- 4112 Off

                default: return (ushort)512;

            }
        }


        /// <summary>
        /// En dependencia de la lectura de un holding register devuelve bool para el desescarche
        /// True ---- Desescarche Activo
        /// False ----- Desescarche Inactivo
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="Entry"></param>
        /// <returns></returns>
        public static bool GetXRThawingResultByModel(string modelName, ushort Entry)
        {
            switch (modelName)
            {
                case "XR100D":
                    if (Entry == (ushort)4883) { return true; }
                    else
                    {
                        return false;
                    }

                default: return true;

            }
        }
        public static bool GetXROnOffResultByModel(string modelName, ushort Entry)
        {
            switch (modelName)
            {
                case "XR100D":
                    if (Entry == (ushort)6501) { return true; }
                    else
                    {
                        return false;
                    }

                default: return true;

            }
        }
        public static ushort GetXROnOffWriteResultByModel(string modelName, bool value)
        {
            switch (modelName)
            {
                case "XR100D":
                    if (value)
                    {
                        return (ushort)4883;
                    }

                    else
                    {
                        return (ushort)1;
                    }

                default: return (ushort)512;

            }
        }
        public static ushort GetXRThawingWriteResultByModel(string modelName, bool value)
        {
            switch (modelName)
            {
                case "XR100D":
                    if (value) return (ushort)4883;
                    else return (ushort)4369;

                default: return (ushort)512;

            }
        }
    }
}
