using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DiagramDesignerEngine
{
	class ProgramsFinder
	{
		private List<LineSegment> ExplodedSegments;
		private ProgramRequirementsTable ProgramRequirements;

		/// <param name="lineSegments"> All line segments forming the diagram </param>
		/// <param name="programRequirements"> The program requirements table used to assign programs </param>
		internal ProgramsFinder(List<LineSegment> lineSegments, ProgramRequirementsTable programRequirements)
		{
			this.ExplodedSegments = (new LineSegmentsExploder(lineSegments)).MergeAndExplodeSegments();
			this.ProgramRequirements = programRequirements;
		}

		internal List<EnclosedProgram> FindPrograms()
		{
			// First, extract UndividableDiagramFragments from the line segments, each representing a room
			var dividableFragments = FragmentFactory.MakeFragments(this.ExplodedSegments).Cast<DiagramFragment>().ToList();
			Stack<DiagramFragment> fragmentsToResolve = new Stack<DiagramFragment>(dividableFragments);
			List<UndividableDiagramFragment> extractedFragments = new List<UndividableDiagramFragment>();

			while (fragmentsToResolve.Count > 0)
			{
				var ftr = fragmentsToResolve.Pop();
				
				if (ftr is DividableDiagramFragment)
				{
					var dividedFtr = ((DividableDiagramFragment)ftr).DivideIntoSmallerFragments();
					foreach (DiagramFragment df in dividedFtr)
					{
						fragmentsToResolve.Push(df);
					}
				}
				else if (ftr is UndividableDiagramFragment)
				{
					extractedFragments.Add((UndividableDiagramFragment) ftr);
				}
			}

			// Second, assign the programs

			// Third, make program objects 
			var assignedPrograms = new List<EnclosedProgram>();
			foreach (UndividableDiagramFragment udf in extractedFragments)
			{
				assignedPrograms.Add(new EnclosedProgram(udf));
			}

			return assignedPrograms;
		}
	}
}
