using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeGrammarEngine
{
	/// <summary>
	/// Represents a connection between two nodes in a shape definition. 
	/// Each node is represnted by a label in the form of a positive integer
	/// LabelOfFirstNode is always smaller or equal to LabelOfSecondNode
	/// </summary>
	public readonly struct Connection : IEquatable<Connection>
	{
		public readonly int LabelOfFirstNode;
		public readonly int LabelOfSecondNode;

		public Connection(int label1, int label2)
		{
			if (label1 < 0 || label2 < 0)
			{
				throw new ArgumentException("a label cannot be negative");
			}
			if (label1 <= label2)
			{
				this.LabelOfFirstNode = label1;
				this.LabelOfSecondNode = label2;
			}
			else
			{
				this.LabelOfFirstNode = label2;
				this.LabelOfSecondNode = label1;
			}
		}

		public static bool operator ==(Connection lhs, Connection rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Connection lhs, Connection rhs) => !(lhs == rhs);

		public override bool Equals(object obj) => obj is Connection other && this.Equals(other);
		bool IEquatable<Connection>.Equals(Connection other) => this.Equals(other);

		private bool Equals(Connection s)
		{
			if (this.LabelOfFirstNode == s.LabelOfFirstNode && this.LabelOfSecondNode == s.LabelOfSecondNode)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (this.LabelOfFirstNode, this.LabelOfSecondNode).GetHashCode();
		}
	}
}
