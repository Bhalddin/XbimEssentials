using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
using static Xbim.Ifc4.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Kernel
{
	public partial class IfcProduct : IExpressValidatable
	{
		public enum IfcProductClause
		{
			PlacementForShapeRepresentation,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcProductClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcProductClause.PlacementForShapeRepresentation:
						retVal = (EXISTS(Representation) && EXISTS(ObjectPlacement)) || (EXISTS(Representation) && (SIZEOF(Representation.Representations.Where(temp => TYPEOF(temp).Contains("IFC4.IFCSHAPEREPRESENTATION"))) == 0)) || (!(EXISTS(Representation)));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcProduct");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcProduct.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcProductClause.PlacementForShapeRepresentation))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProduct.PlacementForShapeRepresentation", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}