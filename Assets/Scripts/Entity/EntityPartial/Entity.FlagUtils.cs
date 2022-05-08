using UnityEngine;



public abstract partial class Entity
{
    protected static class FlagUtils
    {
        #region Conditional

        /// <summary>
        /// Achtung!!!!!!!!!!!!! It works as same as bool && bool && bool ...
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool State_Has(EntityState current, params EntityState[] flags)
        {
            EntityState combined = CombineFlags(flags);
            return (current & combined) == combined;
        }
        public static bool State_HasAnyOf(EntityState current, params EntityState[] flags)
        {
            EntityState combined = CombineFlags(flags);
            return (current & combined) != 0;
        }
        #endregion
        public static EntityState State_Remove(EntityState current, params EntityState[] flags)
        {
            EntityState combined = CombineFlags(flags);
            return current & ~combined;
        }

        private static EntityState CombineFlags(EntityState[] flags)
        {
            EntityState combined = 0;
            if (flags.Length == 0)
            {
                throw new System.ArgumentException();
            }
            foreach (EntityState singleFlag in flags)
            {
                combined |= singleFlag;
            }

            return combined;
        }

        public static EntityState State_Add(EntityState current, params EntityState[] flags)
        {
            EntityState combined = CombineFlags(flags);
            return current | combined;
        }
    }
}
