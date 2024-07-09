using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaPathFinderTesting;

namespace TerrariaPathFinderTesing {
	public static class MathOperations {
		private static readonly Dictionary<char, OperationID> CharToOperation = new() {
			{ '+', OperationID.Add },
			{ '-', OperationID.Subtract },
			{ '*', OperationID.Multiply },
			{ '/', OperationID.Divide },
			{ '^', OperationID.Power },
		};
		private static readonly Dictionary<string, PrecursorOperationID> StringToPrecursorOperation = new() {
			{ "log", PrecursorOperationID.Log }
		};
		private static readonly Dictionary<string, double> Constants = new() {
			{ "pi", Math.PI },
			{ "e", System.Math.E },
		};
		private static readonly Dictionary<char, double> SymbolConstants = new() {
			{ 'π', Math.PI },
		};
		public static Value? GetEquationValue(this string s) {
			try {
				Value? tree = null;
				PrecursorOperationID precursorOperation = PrecursorOperationID.None;
				for (int i = 0; i < s.Length;) {
					char c = s[i];
					switch (c) {
						case ' ':
							i++;
							continue;
						case ',':
							//Need to create 2 fake parentheses for precursor operators and check when a comma is used to swap between the 2 fake inner parentheses.
							//It will return the left value instead of itself and swap to the right value on a comma.
							if (tree == null)
								throw new Exception("Failed to parse string.  The tree was null at a comma.");

							tree = tree.OnComma();
							i++;
							continue;
						case '(':
							if (precursorOperation != PrecursorOperationID.None) {
								ApplyPrecursorOperation(ref tree, precursorOperation);
								precursorOperation = PrecursorOperationID.None;
							}
							else {
								Parenthesis parenthesis = new Parenthesis();
								if (tree == null) {
									tree = parenthesis;
								}
								else {
									tree = tree.Join(parenthesis);
								}
							}

							i++;
							continue;
						case ')':
							if (tree == null)
								throw new Exception("Failed to close parenthesis.  The tree was null.");

							tree = tree.CloseParenthesis();
							i++;
							continue;
					}

					if (TryGetOperator(s, ref i, ref tree)) {
						continue;
					}

					if (TryGetConstant(s, ref i, ref tree)) {
						continue;
					}

					if (CheckStringForWords(s, ref i, ref tree, ref precursorOperation)) {
						continue;
					}

					throw new Exception($"Failed to parse string at index {i}: {s} (remaining: {s.Substring(i)})");
				}

				return tree?.GetTreeHead();
			}
			catch (Exception e) {
				e.ToString().Log();
				return null;
			}
		}
		private static readonly SortedSet<char> NotAllowedInVariableName = new SortedSet<char>(CharToOperation.Keys.Concat(SymbolConstants.Keys).Append('(').Append(')').Append(' ').Append(','));
		private static bool CheckStringForWords(string s, ref int i, ref Value? tree, ref PrecursorOperationID precursorOperation) {
			char firstChar = s[i];
			if (SymbolConstants.TryGetValue(firstChar, out double symbolConstant)) {
				PlaceConstantOrVariable(ref tree, new NamedConstantValue(firstChar.ToString(), symbolConstant));
				i++;
				return true;
			}

			string word = "";
			while (i < s.Length) {
				char c = s[i];
				if (c == ' ') {
					i++;
					continue;
				}

				if (Char.IsLetter(c)) {
					word += c;
					i++;
				}
				else {
					break;
				}
			}

			if (Constants.TryGetValue(word, out double value)) {
				PlaceConstantOrVariable(ref tree, new NamedConstantValue(word, value));
				return true;
			}

			if (StringToPrecursorOperation.TryGetValue(word, out PrecursorOperationID operationID)) {
				if (precursorOperation != PrecursorOperationID.None)
					throw new Exception("Failed to parse string.  There was already a precursor operation.");

				precursorOperation = operationID;
				return true;
			}

			while (i < s.Length) {
				char c = s[i];
				if (c == ' ') {
					i++;
					continue;
				}

				if (!NotAllowedInVariableName.Contains(c)) {
					word += c;
					i++;
				}
				else {
					break;
				}
			}

			if (word.Length == 0) {
				throw new Exception($"Failed to Extract a value from the string: {s}");
			}

			ValueReference valueReference = new ValueReference(MathOperationsTesting.StringHandler, word);
			PlaceConstantOrVariable(ref tree, valueReference);
			return true;
		}
		private static bool TryGetConstant(string s, ref int i, ref Value? tree) {
			int start = i;
			int end = i;
			while (end < s.Length) {
				char c = s[end];
				if (Char.IsDigit(c) || c == '.') {
					end++;
				}
				else {
					break;
				}
			}

			string valueString = s.Substring(start, end - start);

			if (double.TryParse(valueString, out double value)) {
				ConstantValue constant = new ConstantValue(value);
				PlaceConstantOrVariable(ref tree, constant);
				i = end;
				return true;
			}

			return false;
		}

		private static bool TryGetOperator(string s, ref int i, ref Value? tree) {
			if (CharToOperation.TryGetValue(s[i], out OperationID mathOperation)) {
				i++;
				ApplyOperation(ref tree, mathOperation);

				return true;
			}

			return false;
		}
		private static void ApplyOperation(ref Value? tree, OperationID operationID) {
			if (operationID == OperationID.None)
				throw new Exception("Failed to apply operation.  The operationID is None.");

			if (tree == null)
				throw new Exception("Failed to apply operation.  The last Value is null.");

			Operation operation = new Operation(operationID);
			tree = tree.Join(operation);
		}
		private static void ApplyPrecursorOperation(ref Value? tree, PrecursorOperationID operationID) {
			if (operationID == PrecursorOperationID.None)
				throw new Exception("Failed to apply precursor operation.  The PrecursorOperationID is None.");

			PrecursorOperation operation = new(operationID);
			if (tree == null) {
				tree = operation.Setup();
			}
			else {
				tree = tree.Join(operation);
			}
		}
		private static void PlaceConstantOrVariable(ref Value? tree, DoubleValue constant) {
			if (tree == null) {
				tree = constant;
			}
			else {
				tree = tree.Join(constant);
			}
		}
		public static string AddSpaces(this string s) => s;//TODO: Delete this.
		public static string GetString(this OperationID operationID, string? left, string? right) {
			switch (operationID) {
				case OperationID.Add:
					return $"{left} + {right}";
				case OperationID.Subtract:
					return $"{left} - {right}";
				case OperationID.Multiply:
					return $"{left} * {right}";
				case OperationID.Divide:
					return $"{left} / {right}";
				case OperationID.Power:
					return $"{left}^{right}";
				default:
					throw new Exception($"Failed to get string for OperationID: {operationID}");
			}
		}
		public static string GetString(this PrecursorOperationID operationID, string? left, string? right) {
			switch (operationID) {
				case PrecursorOperationID.Log:
					return $"log({left}, {right})";
				default:
					throw new Exception($"Failed to get string for OperationID: {operationID}");
			}
		}
	}

	public enum OperationID {
		None,
		Add,
		Subtract,
		Multiply,
		Divide,
		Power,
	}
	public enum PrecursorOperationID {
		None,
		Log,
	}
	public abstract class Value {
		public ParentValue? Parent;
		public abstract double GetValue();
		internal Value? GetTreeHead() {
			Value? current = this;
			while (current.Parent != null) {
				current = current.Parent;
			}

			return current;
		}

		public Value(ParentValue? parent) {
			Parent = parent;
		}
		public Value? CloseParenthesis() {
			Value value = this;
			while (value.Parent != null && value.Parent is not Parenthesis) {
				value = value.Parent;
			}

			if (value.Parent is Parenthesis parenthesis) {
				parenthesis.InnerValue = value;
				return parenthesis.Close();
			}

			throw new Exception($"Failed to close parenthesis for {this}.  No parent parenthesis found.");
		}
		public Value? Join(Value value) {
			if (value is DoubleValue doubleValue) {
				return Join(doubleValue);
			}
			else if (value is Parenthesis parenthesis) {
				return Join(parenthesis);
			}
			else if (value is Operation operation) {
				return Join(operation);
			}
			else if (value is PrecursorOperation precursorOperation) {
				return Join(precursorOperation);
			}
			else {
				throw new Exception($"Unknown value type: {value}");
			}	
		}
		public virtual Value? Join(Parenthesis parenthesis) =>
			throw new Exception($"Cannot join Parenthesis onto {this}.");
		public virtual Value? Join(Operation operation) =>
			throw new Exception($"Cannot join Operation onto {this}.");
		public virtual Value? Join(DoubleValue value) =>
			throw new Exception($"Cannot join Value onto {this}.");
		public virtual Value? Join(PrecursorOperation precursorOperation) =>
			throw new Exception($"Cannot join PrecursorOperation onto {this}.");
		protected Value? MyselfAsLeftInputForOperation(Operation operation) {
			if (operation.Left != null)
				throw new Exception($"Failed to join operation onto Parenthesis because operation.Left wasn't null");

			operation.Parent = Parent;
			Parent = operation;
			operation.Left = this;
			return operation;
		}
		internal Value? OnComma() {
			Value? value = CloseParenthesis();
			if (value == null)
				throw new Exception($"OnComma; Failed to close parenthesis for {this}.  CloseParenthesis resulted in null.");

			if (value is not PrecursorOperation precursorOperation)
				throw new Exception($"OnComma failed because tree value isn't a PrecursorOperation.");

			if (precursorOperation.Right != null)
				throw new Exception($"On Comma failed to move to the right side of precursorOperation: {precursorOperation} because precursorOperation.Right wasn't null");

			return precursorOperation.TransferToRight();
		}
	}
	public abstract class DoubleValue : Value {
		public DoubleValue(ParentValue? parent) : base(parent) {}
		public override Value? Join(Operation operation) =>
			MyselfAsLeftInputForOperation(operation);
	}
	public abstract class ParentValue : Value {
		protected ParentValue(ParentValue? parent) : base(parent) {}
	}
	public class ConstantValue : DoubleValue {
		private double value;
		public ConstantValue(double value, Operation? parent = null) : base(parent) {
			this.value = value;
		}
		public override double GetValue() => value;
		public override string ToString() => value.ToString("F2");
	}
	public class ValueReference : DoubleValue {
		private Func<double> valueGetter;
		private string referenceName;
		public ValueReference(Func<string, Func<double>> stringHander, string s, Operation? parent = null) : this(stringHander(s), s, parent) { }
		public ValueReference(Func<double> valueGetter, string s, Operation? parent = null) : base(parent) {
			this.valueGetter = valueGetter;
			referenceName = s;
		}
		public override double GetValue() => valueGetter();
		public override string ToString() => referenceName.AddSpaces();
	}
	public class NamedConstantValue : DoubleValue {
		private string name;
		private double value;
		public NamedConstantValue(string name, double value, Operation? parent = null) : base(parent) {
			this.name = name;
			this.value = value;
		}
		public override double GetValue() => value;
		public override string ToString() => name;
	}
	public class Operation : ParentValue {
		public Value? Left = null;
		public Value? Right = null;
		public readonly OperationID OperationID;
		private Func<double, double, double>? func;
		public Operation(OperationID operation, Value? left = null, Operation? parent = null) : base(parent) {
			Left = left;
			OperationID = operation;
			SetFunction();
		}
		private static double Add(double a, double b) => a + b;
		private static double Subtract(double a, double b) => a - b;
		private static double Multiply(double a, double b) => a * b;
		private static double Divide(double a, double b) => a / b;
		private void SetFunction() {
			switch (OperationID) {
				case OperationID.Add:
					func = Add;
					break;
				case OperationID.Subtract:
					func = Subtract;
					break;
				case OperationID.Multiply:
					func = Multiply;
					break;
				case OperationID.Divide:
					func = Divide;
					break;
				case OperationID.Power:
					func = Math.Pow;
					break;
				default:
					throw new Exception($"Unknown operation: {OperationID}");
			}
		}
		public override double GetValue() => func?.Invoke(Left?.GetValue() ?? -13.13, Right?.GetValue() ?? -13.13d) ?? -13.13d;
		public override string ToString() => OperationID.GetString(Left?.ToString() ?? "none", Right?.ToString() ?? "none");
		public override Value? Join(Parenthesis parenthesis) {
			Right = parenthesis;
			parenthesis.Parent = this;
			return parenthesis;
		}
		public override Value? Join(Operation operation) {
			if (Right != null && OperationID < operation.OperationID) {
				//Set new operation as child (right)
				operation.Left = Right;
				Right = operation;
				operation.Parent = this;
				return operation;
			}
			else {
				//Swap this operation with the new operation.  This becomes the left of the new operation.
				operation.Left = this;
				operation.Parent = Parent;
				Parent = operation;
				return operation;
			}
		}
		public override Value? Join(DoubleValue value) {
			Right = value;
			value.Parent = this;
			return this;
		}
		public override Value? Join(PrecursorOperation precursorOperation) {
			Right = precursorOperation;
			precursorOperation.Parent = this;
			return precursorOperation.Setup();
		}
	}
	public class PrecursorOperation : ParentValue {
		public Value? Left = null;
		public Value? Right = null;
		public readonly PrecursorOperationID OperationID;
		private Func<double, double, double>? func;
		public PrecursorOperation(PrecursorOperationID operation, Operation? parent = null) : base(parent) {
			OperationID = operation;
			SetFunction();
		}
		private void SetFunction() {
			switch (OperationID) {
				case PrecursorOperationID.Log:
					func = Math.Log;
					break;
				default:
					throw new Exception($"Unknown operation: {OperationID}");
			}
		}
		public override double GetValue() => func?.Invoke(Left?.GetValue() ?? -13.13, Right?.GetValue() ?? -13.13d) ?? -13.13d;
		public override string ToString() => OperationID.GetString(Left?.ToString() ?? "none", Right?.ToString() ?? "none");
		public override Value? Join(Operation operation) {
			if (Left == null || Right == null)
				throw new Exception($"Failed to join operation onto {this} because Left or Right was null.");

			return MyselfAsLeftInputForOperation(operation);
		}
		internal Value? Setup() {
			Left = new FakeParenthesis();
			Left.Parent = this;
			return Left;
		}
		internal Value? TransferToRight() {
			Right = new FakeParenthesis();
			Right.Parent = this;
			return Right;
		}
	}
	public class Parenthesis : ParentValue {
		public Value? InnerValue = null;
		public Parenthesis(Value? innerValue = null, Operation? parent = null) : base(parent) {
			InnerValue = innerValue;
		}
		public override double GetValue() => InnerValue?.GetValue() ?? -13.13d;
		public override string ToString() => $"({InnerValue?.ToString() ?? "none"})";
		public override Value? Join(Operation operation) {
			if (InnerValue == null)
				throw new Exception($"Can't treat Parenthesis as a constant value input to the operation {operation} because InnerValue is null.");

			return MyselfAsLeftInputForOperation(operation);
		}
		public override Value? Join(PrecursorOperation precursorOperation) {
			precursorOperation.Parent = this;
			return precursorOperation.Setup();
		}
		public override Value? Join(Parenthesis parenthesis) => JoinCommon(parenthesis);
		public override Value? Join(DoubleValue value) => JoinCommon(value);
		private Value? JoinCommon(Value value) {
			value.Parent = this;
			return value;
		}

		internal virtual Value? Close() {
			return this;
		}
	}
	public class FakeParenthesis : Parenthesis {
		internal override Value? Close() {
			if (InnerValue == null)
				throw new Exception("Can't close FakeParenthesis because InnerValue is null.");

			if (Parent is not PrecursorOperation precursorOperation)
				throw new Exception("Can't close FakeParenthesis because Parent is not a PrecursorOperation.");

			if (precursorOperation.Left == this) {
				precursorOperation.Left = InnerValue;
			}
			else if (precursorOperation.Right == this) {
				precursorOperation.Right = InnerValue;
			}
			else {
				throw new Exception("Can't close FakeParenthesis because Parent doesn't have this as a child.");
			}

			InnerValue.Parent = Parent;

			return Parent;
		}
	}
}