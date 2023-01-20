using System;
using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public class Interpreter
    {
        Parser m_Parser;

        public Interpreter(Parser parser)
        {
            m_Parser = parser;
        }

        void Error(string reason)
        {
            throw new Exception("Semantic Error: " + reason + "\nIn field string: " + m_Parser.GetText());
        }

        FieldPosition VisitAtom(AtomNode atom, FieldType type = FieldType.UNSPECIFIED)
        {
            FieldPosition position = new FieldPosition();
            if (atom.Type == NodeType.NAME || atom.Type == NodeType.STRING)
            {
                position.Type = FieldType.KEY;
                position.Key = atom.Value;
            }
            else if (atom.Type == NodeType.INTEGER)
            {
                if (type == FieldType.KEY)
                {
                    position.Type = FieldType.KEY;
                    position.Key = atom.Value;
                }
                else
                {
                    position.Type = FieldType.INDEX;
                    position.Index = Convert.ToInt32(atom.Value);
                }
            }
            else Error(atom.Value);
            return position;
        }

        FieldPosition VisitPosition(SliceNode slice, FieldType type = FieldType.UNSPECIFIED)
        {
            // You have to make sure this is atom first (slice.IsAtom = true)
            if (slice.IsAtom) return VisitAtom(slice.Start, type);
            else Error("This is a slice");
            return null;
        }

        FieldSlice VisitSlice(SliceNode slice, FieldType type = FieldType.UNSPECIFIED)
        {
            // You have to make sure this is slice first (slice.IsAtom = false)
            // If contain both key and index type, will convert to key type
            FieldSlice fieldSlice = new FieldSlice();
            if (!slice.IsAtom)
            {
                FieldPosition start = null;
                if (slice.Start != null) start = VisitAtom(slice.Start, type);
                FieldPosition end = null;
                if (slice.End != null) end = VisitAtom(slice.End, type);
                FieldPosition step = null;
                if (slice.Step != null) step = VisitAtom(slice.Step, type);
                
                if (start == null && end == null)
                {
                    fieldSlice.Type = type;
                }
                else if (start != null && end == null)
                {
                    fieldSlice.Type = start.Type;
                    fieldSlice.HasStart = true;
                    if (start.Type == FieldType.INDEX) fieldSlice.StartIndex = start.Index;
                    else fieldSlice.StartKey = slice.Start.Value;
                }
                else if (start == null && end != null)
                {
                    fieldSlice.Type = end.Type;
                    fieldSlice.HasEnd = true;
                    if (end.Type == FieldType.INDEX) fieldSlice.EndIndex = end.Index;
                    else fieldSlice.EndKey = slice.End.Value;
                }
                else
                {
                    fieldSlice.HasStart = true;
                    fieldSlice.HasEnd = true;
                    if (start.Type == FieldType.INDEX && end.Type == FieldType.INDEX)
                    {
                        fieldSlice.Type = FieldType.INDEX;
                        fieldSlice.StartIndex = start.Index;
                        fieldSlice.EndIndex = end.Index;
                    }
                    else
                    {
                        fieldSlice.Type = FieldType.KEY;
                        fieldSlice.StartKey = slice.Start.Value;
                        fieldSlice.EndKey = slice.End.Value;
                    }
                }

                if (step == null) fieldSlice.Step = 1;
                else if (step.Type == FieldType.INDEX) fieldSlice.Step = step.Index;
                else Error("The step " + slice.Step.Value + " of the slice is not integer");
            }
            else Error("This is not a slice");
            return fieldSlice;
        }

        FieldPath VisitPath(MappingNode mapping)
        {
            // The position can only be single value
            if (mapping.Dimensions.Count == 1)
            {
                if (mapping.Dimensions[0].Assignments.Count == 1)
                {
                    if (mapping.Dimensions[0].Assignments[0].Right == null)
                    {
                        SliceNode slice = mapping.Dimensions[0].Assignments[0].Left;
                        if (slice != null)
                        {
                            if (slice.IsAtom)
                            {
                                FieldPath path = new FieldPath();
                                FieldPosition position = VisitPosition(slice);
                                if (mapping.Type == NodeType.SEQUENCE)
                                {
                                    if (position.Type == FieldType.KEY) path.Type = FieldType.DICT;
                                    else path.Type = FieldType.SEQUENCE;
                                }
                                else if (mapping.Type == NodeType.TUPLE)
                                {
                                    if (position.Type == FieldType.KEY) path.Type = FieldType.DICT;
                                    else path.Type = FieldType.TUPLE;
                                }
                                else if (mapping.Type == NodeType.DICT)
                                {
                                    position = VisitPosition(slice, FieldType.KEY);
                                    path.Type = FieldType.DICT;
                                }
                                else Error("It is not Sequence, Tuple or Dict");
                                path.Position = position;
                                return path;
                            }
                            else Error("The key or index in the middle cannot be slice");
                        }
                        else Error("The key or index in the middle cannot be null");
                    }
                    else Error("The key or index in the middle cannot be assignment");
                }
                else Error("The key or index in the middle can only be singular");
            }
            else Error("The key or index in the middle can only be singular");
            return null;
        }

        FieldAssignment VisitAssignment(AssignmentNode assignment, FieldType type = FieldType.UNSPECIFIED)
        {
            // Not checking the length matching or not, because some unknown parameters
            FieldAssignment fieldAssignment = new FieldAssignment();

            if (assignment.Left != null)
            {
                if (assignment.Left.IsAtom)
                {
                    FieldPosition position = VisitPosition(assignment.Left, type);
                    FieldSlice destination = new FieldSlice();
                    destination.HasStart = true;
                    destination.Step = 0;
                    if (position.Type == FieldType.INDEX)
                    {
                        destination.Type = FieldType.INDEX;
                        destination.StartIndex = position.Index;
                    }
                    else if (position.Type == FieldType.KEY)
                    {
                        destination.Type = FieldType.KEY;
                        destination.StartKey = position.Key;
                    }
                    else Error("Wrong position type");
                    fieldAssignment.Destination = destination;
                }
                else fieldAssignment.Destination = VisitSlice(assignment.Left, type);
            }
            else Error("lvalue cannot be null");

            if (assignment.Right != null)
            {
                if (assignment.Right.IsAtom)
                {
                    FieldPosition position = VisitPosition(assignment.Right);
                    FieldSlice source = new FieldSlice();
                    source.HasStart = true;
                    source.Step = 0;
                    if (position.Type == FieldType.INDEX)
                    {
                        source.Type = FieldType.INDEX;
                        source.StartIndex = position.Index;
                    }
                    else if (position.Type == FieldType.KEY)
                    {
                        source.Type = FieldType.KEY;
                        source.StartKey = position.Key;
                    }
                    else Error("Wrong position type");
                    fieldAssignment.Source = source;
                }
                else fieldAssignment.Source = VisitSlice(assignment.Right);
            }
            return fieldAssignment;
        }

        FieldCollection VisitCollection(AssignmentsNode dimension, FieldType type = FieldType.UNSPECIFIED)
        {
            FieldCollection collection = new FieldCollection();
            foreach (AssignmentNode assignment in dimension.Assignments)
            {
                collection.Assignments.Add(VisitAssignment(assignment, type));
            }
            return collection;
        }

        FieldMapping VisitMapping(MappingNode mapping)
        {
            // Convert index to key if they both exists
            FieldType finalType = FieldType.INDEX;
            if (mapping.Type == NodeType.DICT) finalType = FieldType.KEY;
            foreach (AssignmentsNode dimension in mapping.Dimensions)
            {
                if (finalType == FieldType.KEY) break;
                foreach (AssignmentNode assignment in dimension.Assignments)
                {
                    SliceNode slice = assignment.Left;
                    if (slice.IsAtom)
                    {
                        if (VisitPosition(slice).Type == FieldType.KEY)
                        {
                            finalType = FieldType.KEY;
                            break;
                        }
                    }
                    else
                    {
                        if (VisitSlice(slice).Type == FieldType.KEY)
                        {
                            finalType = FieldType.KEY;
                            break;
                        }
                    }
                }
            }

            // Main part
            if (mapping.Dimensions.Count > 0)
            {
                FieldMapping fieldMapping = new FieldMapping();

                if (finalType == FieldType.KEY) fieldMapping.Type = FieldType.DICT;
                else if (mapping.Type == NodeType.SEQUENCE) fieldMapping.Type = FieldType.SEQUENCE;
                else if (mapping.Type == NodeType.TUPLE) fieldMapping.Type = FieldType.TUPLE;
                else { Error("Wrong Mapping type"); return null; }

                if (fieldMapping.Type == FieldType.DICT && mapping.Dimensions.Count > 1)
                {
                    Error("Dict can only have 1 dimension");
                    return null;
                }

                foreach (AssignmentsNode dimension in mapping.Dimensions)
                {
                    fieldMapping.Dimensions.Add(VisitCollection(dimension, finalType));
                }

                return fieldMapping;
            }
            else Error("Mapping dimension must > 1");
            return null;
        }

        FieldString VisitField(Node node)
        {
            FieldString fieldString = new FieldString();
            if (node.Agents != null) fieldString.IsAllAgents = node.Agents.IsAll;
            else fieldString.IsAllAgents = false;
            if (node.Agents != null) fieldString.Agents = node.Agents.Agents;
            fieldString.IsRoot = node.IsRoot;
            Queue<MappingNode> mappings = new Queue<MappingNode>();
            foreach (MappingNode mapping in node.Mappings)
            {
                for (int i = 0; i < mapping.Upper; i++)
                {
                    if (mappings.Count == 0) fieldString.Upper++;
                    else mappings.Dequeue();
                }
                mappings.Enqueue(mapping);
            }
            while (mappings.Count > 1)
            {
                fieldString.Paths.Add(VisitPath(mappings.Dequeue()));
            }
            if (mappings.Count == 1)
            {
                fieldString.Mapping = VisitMapping(mappings.Dequeue());
            }
            if (fieldString.IsRoot) fieldString.Upper = 0;
            return fieldString;
        }

        public FieldString Interpret()
        {
            Node tree = m_Parser.Parse();
            return VisitField(tree);
        }
    }
}