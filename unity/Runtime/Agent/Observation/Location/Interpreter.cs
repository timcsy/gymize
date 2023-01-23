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
            throw new Exception("Semantic Error: " + reason + "\nIn location string: " + m_Parser.GetText());
        }

        Selector VisitAtom(AtomNode atom, LocationType type = LocationType.UNSPECIFIED)
        {
            Selector selector = new Selector();
            if (atom.Type == NodeType.NAME || atom.Type == NodeType.STRING)
            {
                selector.Type = LocationType.KEY;
                selector.Key = atom.Value;
            }
            else if (atom.Type == NodeType.INTEGER)
            {
                if (type == LocationType.KEY)
                {
                    selector.Type = LocationType.KEY;
                    selector.Key = atom.Value;
                }
                else
                {
                    selector.Type = LocationType.INDEX;
                    selector.Index = Convert.ToInt32(atom.Value);
                }
            }
            else Error(atom.Value);
            return selector;
        }

        Selector VisitSelector(SliceNode slice, LocationType type = LocationType.UNSPECIFIED)
        {
            // You have to make sure this is atom first (slice.IsAtom = true)
            if (slice.IsAtom) return VisitAtom(slice.Start, type);
            else Error("This is a slice");
            return null;
        }

        Slice VisitSlice(SliceNode slice, LocationType type = LocationType.UNSPECIFIED)
        {
            // You have to make sure this is slice first (slice.IsAtom = false)
            // If contain both key and index type, will convert to key type
            Slice _slice = new Slice();
            if (!slice.IsAtom)
            {
                Selector start = null;
                if (slice.Start != null) start = VisitAtom(slice.Start, type);
                Selector end = null;
                if (slice.End != null) end = VisitAtom(slice.End, type);
                Selector step = null;
                if (slice.Step != null) step = VisitAtom(slice.Step, type);
                
                if (start == null && end == null)
                {
                    _slice.Type = type;
                }
                else if (start != null && end == null)
                {
                    _slice.Type = start.Type;
                    _slice.HasStart = true;
                    if (start.Type == LocationType.INDEX) _slice.StartIndex = start.Index;
                    else _slice.StartKey = slice.Start.Value;
                }
                else if (start == null && end != null)
                {
                    _slice.Type = end.Type;
                    _slice.HasEnd = true;
                    if (end.Type == LocationType.INDEX) _slice.EndIndex = end.Index;
                    else _slice.EndKey = slice.End.Value;
                }
                else
                {
                    _slice.HasStart = true;
                    _slice.HasEnd = true;
                    if (start.Type == LocationType.INDEX && end.Type == LocationType.INDEX)
                    {
                        _slice.Type = LocationType.INDEX;
                        _slice.StartIndex = start.Index;
                        _slice.EndIndex = end.Index;
                    }
                    else
                    {
                        _slice.Type = LocationType.KEY;
                        _slice.StartKey = slice.Start.Value;
                        _slice.EndKey = slice.End.Value;
                    }
                }

                if (step == null) _slice.Step = 1;
                else if (step.Type == LocationType.INDEX) _slice.Step = step.Index;
                else Error("The step " + slice.Step.Value + " of the slice is not integer");
            }
            else Error("This is not a slice");
            return _slice;
        }

        Path VisitPath(MappingNode mapping)
        {
            // The selector can only be single value
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
                                Path path = new Path();
                                Selector selector = VisitSelector(slice);
                                if (mapping.Type == NodeType.SEQUENCE)
                                {
                                    if (selector.Type == LocationType.KEY) path.Type = LocationType.DICT;
                                    else path.Type = LocationType.SEQUENCE;
                                }
                                else if (mapping.Type == NodeType.TUPLE)
                                {
                                    if (selector.Type == LocationType.KEY) path.Type = LocationType.DICT;
                                    else path.Type = LocationType.TUPLE;
                                }
                                else if (mapping.Type == NodeType.DICT)
                                {
                                    selector = VisitSelector(slice, LocationType.KEY);
                                    path.Type = LocationType.DICT;
                                }
                                else Error("It is not Sequence, Tuple or Dict");
                                path.Selector = selector;
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

        Assignment VisitAssignment(AssignmentNode assignment, LocationType type = LocationType.UNSPECIFIED)
        {
            // Not checking the length matching or not, because some unknown parameters
            Assignment _assignment = new Assignment();

            if (assignment.Left != null)
            {
                if (assignment.Left.IsAtom)
                {
                    Selector selector = VisitSelector(assignment.Left, type);
                    Slice destination = new Slice();
                    destination.HasStart = true;
                    destination.Step = 0;
                    if (selector.Type == LocationType.INDEX)
                    {
                        destination.Type = LocationType.INDEX;
                        destination.StartIndex = selector.Index;
                    }
                    else if (selector.Type == LocationType.KEY)
                    {
                        destination.Type = LocationType.KEY;
                        destination.StartKey = selector.Key;
                    }
                    else Error("Wrong selector type");
                    _assignment.Destination = destination;
                }
                else _assignment.Destination = VisitSlice(assignment.Left, type);
            }
            else Error("lvalue cannot be null");

            if (assignment.Right != null)
            {
                if (assignment.Right.IsAtom)
                {
                    Selector selector = VisitSelector(assignment.Right);
                    Slice source = new Slice();
                    source.HasStart = true;
                    source.Step = 0;
                    if (selector.Type == LocationType.INDEX)
                    {
                        source.Type = LocationType.INDEX;
                        source.StartIndex = selector.Index;
                    }
                    else if (selector.Type == LocationType.KEY)
                    {
                        source.Type = LocationType.KEY;
                        source.StartKey = selector.Key;
                    }
                    else Error("Wrong selector type");
                    _assignment.Source = source;
                }
                else _assignment.Source = VisitSlice(assignment.Right);
            }
            return _assignment;
        }

        Dimension VisitDimension(AssignmentsNode dimension, LocationType type = LocationType.UNSPECIFIED)
        {
            Dimension _dimension = new Dimension();
            foreach (AssignmentNode assignment in dimension.Assignments)
            {
                _dimension.Assignments.Add(VisitAssignment(assignment, type));
            }
            return _dimension;
        }

        Mapping VisitMapping(MappingNode mapping)
        {
            // Convert index to key if they both exists
            LocationType finalType = LocationType.INDEX;
            if (mapping.Type == NodeType.DICT) finalType = LocationType.KEY;
            foreach (AssignmentsNode dimension in mapping.Dimensions)
            {
                if (finalType == LocationType.KEY) break;
                foreach (AssignmentNode assignment in dimension.Assignments)
                {
                    SliceNode slice = assignment.Left;
                    if (slice.IsAtom)
                    {
                        if (VisitSelector(slice).Type == LocationType.KEY)
                        {
                            finalType = LocationType.KEY;
                            break;
                        }
                    }
                    else
                    {
                        if (VisitSlice(slice).Type == LocationType.KEY)
                        {
                            finalType = LocationType.KEY;
                            break;
                        }
                    }
                }
            }

            // Main part
            if (mapping.Dimensions.Count > 0)
            {
                Mapping _mapping = new Mapping();

                if (finalType == LocationType.KEY) _mapping.Type = LocationType.DICT;
                else if (mapping.Type == NodeType.SEQUENCE) _mapping.Type = LocationType.SEQUENCE;
                else if (mapping.Type == NodeType.TUPLE) _mapping.Type = LocationType.TUPLE;
                else { Error("Wrong Mapping type"); return null; }

                if (_mapping.Type == LocationType.DICT && mapping.Dimensions.Count > 1)
                {
                    Error("Dict can only have 1 dimension");
                    return null;
                }

                foreach (AssignmentsNode dimension in mapping.Dimensions)
                {
                    _mapping.Dimensions.Add(VisitDimension(dimension, finalType));
                }

                return _mapping;
            }
            else Error("Mapping dimension must > 1");
            return null;
        }

        Location VisitLocation(Node node)
        {
            Location location = new Location();
            if (node.Agents != null) location.IsAllAgents = node.Agents.IsAll;
            else location.IsAllAgents = false;
            if (node.Agents != null) location.Agents = node.Agents.Agents;
            location.IsRoot = node.IsRoot;
            List<MappingNode> mappings = new List<MappingNode>();
            foreach (MappingNode mapping in node.Mappings)
            {
                for (int i = 0; i < mapping.Upper; i++)
                {
                    if (mappings.Count == 0) location.Upper++;
                    else mappings.RemoveAt(mappings.Count - 1);
                }
                mappings.Add(mapping);
            }
            for (int i = 0; i < mappings.Count - 1; i++)
            {
                location.Paths.Add(VisitPath(mappings[i]));
            }
            if (mappings.Count > 0)
            {
                location.Mapping = VisitMapping(mappings[mappings.Count - 1]);
            }
            if (location.IsRoot) location.Upper = 0;
            return location;
        }

        public Location Interpret()
        {
            Node tree = m_Parser.Parse();
            return VisitLocation(tree);
        }
    }
}