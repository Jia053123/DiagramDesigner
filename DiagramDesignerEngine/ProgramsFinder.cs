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
			List<UndividableDiagramFragment> extractedFragments = FragmentFactory.ExtractAllFragments(this.ExplodedSegments);

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
