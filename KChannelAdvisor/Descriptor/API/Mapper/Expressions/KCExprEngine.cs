
using KChannelAdvisor.BLC;
using KChannelAdvisor.Descriptor.API.Mapper.Expressions.ExprHandlers;
using PX.Data;
using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.API.Mapper.Expressions
{
    internal class KCExprEngine
    {
        private static readonly object locker = new object();
        internal static string EntityType { get; set; }

        internal static KCIExprHandler HandlingChain
        {
            get
            {
                KCAddExprHandler addHandler = new KCAddExprHandler();
                KCSubtractExpHandler subtractHandler = new KCSubtractExpHandler();
                KCConstantExprHandler constHandler = new KCConstantExprHandler(EntityType);

                addHandler.SetNext(subtractHandler).SetNext(constHandler);

                return addHandler;
            }
        }

        internal static Dictionary<string, char> Operators { get; } = new Dictionary<string, char>
        {
            { nameof(KCConstants.Add), KCConstants.Add },
            { nameof(KCConstants.Subtract), KCConstants.Subtract },
        };

        internal static KCMappingMaint MappingMaint
        {
            get
            {
                lock (locker)
                {
                    return PXGraph.CreateInstance<KCMappingMaint>();
                }
            }
        }

        public static KCIExpression AssembleExpression(string entityType, string exprFormula)
        {
            EntityType = entityType;
            var expr = HandlingChain.Handle(exprFormula);
            return expr;
        }
    }
}
