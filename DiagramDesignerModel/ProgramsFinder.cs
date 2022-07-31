﻿using BasicGeometries;
using DiagramDesignerGeometryParser;
using System;
using System.Collections.Generic;

namespace DiagramDesignerModel
{
	internal class ProgramsFinder
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
			// at the moment, simply assign the program closest in area
			List<String> programNames = new List<String>();
			for (int i = 0; i < extractedFragments.Count; i++)
			{
				var fragmentArea = extractedFragments[i].CalculateFragmentArea();

				String name;
				double requiredArea;
				if (ProgramRequirements.Rows.Count == 0)
				{
					name = "Unnamed";
					requiredArea = 0;
				}
				else
				{
					name = (String)ProgramRequirements.Rows[0]["Name"];
					requiredArea = (double)ProgramRequirements.Rows[0]["Area"];
				}

				for (int j = 1; j < ProgramRequirements.Rows.Count; j++)
				{
					var currentDiff = Math.Abs(fragmentArea - requiredArea);
					var newDiff = Math.Abs(fragmentArea - (double)ProgramRequirements.Rows[j]["Area"]);
					if (newDiff < currentDiff)
					{
						name = (String)ProgramRequirements.Rows[j]["Name"];
						requiredArea = (double)ProgramRequirements.Rows[j]["Area"];
					}
				}

				programNames.Add(name);
			}

			// Third, make program objects 
			var assignedPrograms = new List<EnclosedProgram>();
			for (int i = 0; i < extractedFragments.Count; i++)
			{
				assignedPrograms.Add(new EnclosedProgram(programNames[i], extractedFragments[i]));
			}

			return assignedPrograms;
		}
	}
}
