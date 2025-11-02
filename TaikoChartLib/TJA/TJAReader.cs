using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TaikoChartLib.TJA
{
    public class TJAReader
    {
        public static readonly Regex RemoveComment = new Regex("( *//.*|\r)", RegexOptions.Multiline);
        public static readonly Regex SplitLineRegex = new Regex("\n");
        public static readonly Regex CommandSplitRegex = new Regex(",");

        private class ParseState
        {
            public Course Course = new Course();
            public float BPM = 150.0f;
            public Vector2 HeadScroll = new Vector2(1.0f, 0.0f);
            public bool Loading;
        }
        private class ParseCourseState
        {
            public int ScoreInit;
            public int ScoreDiff;
            public bool HiddenBranch;
            public ChipsData ChipsData = new ChipsData();
        }
        private class ParseChipsState
        {
            public ChipParams CurrentParams = new ChipParams();
            public ChipParams? PreviousBranchParams;
            public List<QueueChip> ChipQueues = new List<QueueChip>();
        }
        private class QueueChip
        {
            public ChipType ChipType;
            public object? Param;
        }

        public static Difficulty GetDifficulty(string text)
        {
            text = text.ToLower();
            if (text.Contains("easy") || text.Contains("0"))
            {
                return Difficulty.Easy;
            }
            else if (text.Contains("normal") || text.Contains("1"))
            {
                return Difficulty.Normal;
            }
            else if (text.Contains("hard") || text.Contains("2"))
            {
                return Difficulty.Hard;
            }
            else if (text.Contains("oni") || text.Contains("3"))
            {
                return Difficulty.Extreme;
            }
            else if (text.Contains("edit") || text.Contains("4"))
            {
                return Difficulty.Extra;
            }

            return Difficulty.Easy;
        }

        public static TJATaikoChart TJAToChart(string text)
        {
            ParseState state = new ParseState();
            ParseCourseState courseState = new ParseCourseState();
            TJATaikoChart taikoChart = new TJATaikoChart();
            ParseChipsState chipsState = new ParseChipsState();

            text = RemoveComment.Replace(text, "");

            string[] lines = SplitLineRegex.Split(text);
            foreach (string line in lines)
            {
                if (line.Length == 0) continue;

                string[] headerSplited = line.Split(":");
                if (headerSplited.Length >= 2)
                {
                    string key = headerSplited[0];
                    string value = string.Join(null, headerSplited, 1, headerSplited.Length - 1);

                    ParseHeader(taikoChart, ref state, ref courseState, key, value);
                }
                else if (line[0] == '#')
                {
                    ParseCommand(taikoChart, ref state, ref courseState, ref chipsState, line);
                }
                else if (state.Loading)
                {
                    ParseChips(taikoChart, ref state, ref courseState, ref chipsState, line);
                }
            }

            return taikoChart;
        }

        public static Vector2 ParseComplex(string text)
        {
            if (float.TryParse(text, out float value))
            {
                return new Vector2(value, 0.0f);
            }

            return new Vector2(1.0f, 0.0f);
        }

        public static ChipType CharToChipType(char ch)
        {
            switch (ch)
            {
                case '1':
                    return ChipType.Don;
                case '2':
                    return ChipType.Ka;
                case '3':
                    return ChipType.DonBig;
                case '4':
                    return ChipType.KaBig;
                case '5':
                    return ChipType.Roll;
                case '6':
                    return ChipType.RollBig;
                case '7':
                    return ChipType.Balloon;
                case '8':
                    return ChipType.RollEnd;
                case '9':
                    return ChipType.Kusudama;
            }
            return ChipType.None;
        }

        private static void ParseHeader(TJATaikoChart taikoChart, ref ParseState state, ref ParseCourseState courseState, string key, string value)
        {
            switch(key)
            {
                case "TITLE":
                    taikoChart.Title.TryAdd(Lang.Default, value);
                    break;
                case "TITLEEN":
                    taikoChart.Title.TryAdd(Lang.En, value);
                    break;
                case "TITLEJA":
                    taikoChart.Title.TryAdd(Lang.Ja, value);
                    break;
                case "TITLEES":
                    taikoChart.Title.TryAdd(Lang.Es, value);
                    break;
                case "TITLEFR":
                    taikoChart.Title.TryAdd(Lang.Fr, value);
                    break;
                case "TITLEZH":
                    taikoChart.Title.TryAdd(Lang.Zh, value);
                    break;
                case "SUBTITLE":
                    taikoChart.SubTitle.TryAdd(Lang.Default, value);
                    break;
                case "SUBTITLEEN":
                    taikoChart.SubTitle.TryAdd(Lang.En, value);
                    break;
                case "SUBTITLEJA":
                    taikoChart.SubTitle.TryAdd(Lang.Ja, value);
                    break;
                case "SUBTITLEES":
                    taikoChart.SubTitle.TryAdd(Lang.Es, value);
                    break;
                case "SUBTITLEFR":
                    taikoChart.SubTitle.TryAdd(Lang.Fr, value);
                    break;
                case "SUBTITLEZH":
                    taikoChart.SubTitle.TryAdd(Lang.Zh, value);
                    break;
                case "MAKER":
                    foreach (var childValue in CommandSplitRegex.Split(value))
                    {
                        taikoChart.Charter.Add(childValue);
                    }
                    break;
                case "ARTIST":
                    foreach (var childValue in CommandSplitRegex.Split(value))
                    {
                        taikoChart.Artist.Add(childValue);
                    }
                    break;
                case "GENRE":
                    foreach (var childValue in CommandSplitRegex.Split(value))
                    {
                        taikoChart.Genre.Add(childValue);
                    }
                    break;
                case "AUTHOR":
                    taikoChart.Author = value;
                    break;
                case "WAVE":
                    taikoChart.SongFileName = value;
                    break;
                case "PREIMAGE":
                case "COVER":
                    taikoChart.JacketFileName = value;
                    break;
                case "TAIKOWEBSKIN":
                    break;
                case "SCENEPRESET":
                    taikoChart.ScenePreset = value;
                    break;
                case "TOWERTYPE":
                    taikoChart.TowerType = value;
                    break;
                case "SELECTBG":
                    taikoChart.SongSelectionBGFileName = value;
                    break;
                case "BGIMAGE":
                    taikoChart.BGImageFileName = value;
                    break;
                case "BGOFFSET":
                    if (float.TryParse(value, out float bgoffset))
                    {
                        taikoChart.BGImageOffset = bgoffset;
                    }
                    break;
                case "BGMOVIE":
                    taikoChart.BGMovieFileName = value;
                    break;
                case "MOVIEOFFSET":
                    if (float.TryParse(value, out float movieOffset))
                    {
                        taikoChart.BGMovieOffset = movieOffset;
                    }
                    break;
                case "OFFSET":
                    if (float.TryParse(value, out float offset))
                    {
                        taikoChart.SongOffset = offset;
                    }
                    break;
                case "DEMOSTART":
                    if (float.TryParse(value, out float demoStart))
                    {
                        taikoChart.SongPreviewOffset = demoStart;
                    }
                    break;
                case "SONGVOL":
                    if (int.TryParse(value, out int songVol))
                    {
                        taikoChart.SongVolume = songVol;
                    }
                    break;
                case "SEVOL":
                    if (int.TryParse(value, out int seVol))
                    {
                        taikoChart.SoundEffectVolume = seVol;
                    }
                    break;
                case "NOTESDESIGNER":
                case "NOTESDESIGNER1":
                case "NOTESDESIGNER2":
                case "NOTESDESIGNER3":
                case "NOTESDESIGNER4":
                    foreach (var childValue in CommandSplitRegex.Split(value))
                    {
                        state.Course.Charter.Add(childValue);
                    }
                    break;
                case "HEADSCROLL":
                    state.HeadScroll = ParseComplex(value);
                    break;
                case "SIDE":
                    break;
                case "SIDEREV":
                    break;
                case "COURSE":
                    {
                        Difficulty difficulty = GetDifficulty(value);

                        state.Course = new Course();
                        courseState = new ParseCourseState()
                        {
                            ChipsData = new ChipsData()
                        };
                        taikoChart.Courses.TryAdd(difficulty, state.Course);
                    }
                    break;
                case "LEVEL":
                    if (int.TryParse(value, out int level))
                    {
                        state.Course.Level = level;
                    }
                    break;
                case "BPM":
                    if (int.TryParse(value, out int bpm))
                    {
                        state.BPM = bpm;
                    }
                    break;
                case "SCOREINIT":
                    if (int.TryParse(value, out int scoreInit))
                    {
                        courseState.ScoreInit = scoreInit;
                    }
                    break;
                case "SCOREDIFF":
                    if (int.TryParse(value, out int scoreDiff))
                    {
                        courseState.ScoreDiff = scoreDiff;
                    }
                    break;
                case "HIDDENBRANCH":
                    if (int.TryParse(value, out int hiddenBranch))
                    {
                        courseState.HiddenBranch = hiddenBranch >= 1;
                    }
                    break;
                case "TJACOMPAT":
                    if (value.StartsWith("jiro1"))
                    {
                        taikoChart.TJACompat = ~TJACompat.Jiro1;
                    }
                    else if (value.StartsWith("jiro2"))
                    {
                        taikoChart.TJACompat = ~TJACompat.Jiro2;
                    }
                    else if (value.StartsWith("tmg"))
                    {
                        taikoChart.TJACompat = ~TJACompat.TMG;
                    }
                    else if (value.StartsWith("tjap3"))
                    {
                        taikoChart.TJACompat = ~TJACompat.TJAP3;
                    }
                    else if (value.StartsWith("oos"))
                    {
                        taikoChart.TJACompat = ~TJACompat.OOS;
                    }
                    break;
            }
        }

        private static void ParseCommand(TJATaikoChart taikoChart, ref ParseState state, ref ParseCourseState courseState, ref ParseChipsState chipsState, string text)
        {
            if (text.StartsWith("#START"))
            {
                StyleSide side = StyleSide.Single;
                if (text.EndsWith("P1"))
                {
                    side = StyleSide.DoubleP1;
                }
                else if (text.EndsWith("P2"))
                {
                    side = StyleSide.DoubleP2;
                }

                chipsState = new ParseChipsState()
                {
                    CurrentParams = new ChipParams()
                    {
                        BPM = state.BPM,
                        Scroll = state.HeadScroll
                    }
                };

                courseState.ChipsData = new ChipsData()
                {
                    InitBPM = state.BPM,
                    InitScroll = state.HeadScroll
                };

                state.Course.ChipsDatas.TryAdd(side, courseState.ChipsData);

                state.Loading = true;
            }
            else if (text.StartsWith("#END"))
            {
                state.Loading = false;
            }
            else if (text.StartsWith("#BPMCHANGE"))
            {
                if (!float.TryParse(text[10..], out float bpm))
                {
                    bpm = 150.0f;
                }

                QueueChip queueChip = new QueueChip()
                {
                    ChipType = ChipType.BPMChange,
                    Param = bpm
                };
                chipsState.ChipQueues.Add(queueChip);
            }
            else if (text.StartsWith("#SCROLL"))
            {
                Vector2 scroll = new Vector2(1.0f, 0.0f);
                if (!float.TryParse(text[7..], out float scroll_))
                {
                    scroll.X = scroll_;
                }

                QueueChip queueChip = new QueueChip()
                {
                    ChipType = ChipType.Scroll,
                    Param = scroll
                };
                chipsState.ChipQueues.Add(queueChip);
            }
            else if (text.StartsWith("#MEASURE"))
            {
                string[] measureSplited = text[8..].Split('/');
                Vector2 measure = new Vector2(4, 4);

                if (measureSplited.Length >= 1 && float.TryParse(measureSplited[0], out float x))
                {
                    measure.X = x;
                }
                if (measureSplited.Length >= 2 && float.TryParse(measureSplited[1], out float y))
                {
                    measure.Y = y;
                }

                QueueChip queueChip = new QueueChip()
                {
                    ChipType = ChipType.Measure,
                    Param = measure
                };
                chipsState.ChipQueues.Add(queueChip);
            }
        }

        private static void ParseChips(TJATaikoChart taikoChart, ref ParseState state, ref ParseCourseState courseState, ref ParseChipsState chipsState, string text)
        {
            foreach (char ch in text)
            {
                if (ch == ' ') continue;
                else if (ch == ',')
                {
                    int noteCount = chipsState.ChipQueues.Where(x => Chip.IsNote(x.ChipType)).Sum(x => 1);
                    if (noteCount == 0)
                    {
                        QueueChip queueChip = new QueueChip()
                        {
                            ChipType = ChipType.None
                        };
                        chipsState.ChipQueues.Add(queueChip);
                        noteCount++;
                    }

                    float step = 240f / noteCount;


                    foreach (QueueChip queueChip in chipsState.ChipQueues)
                    {
                        switch (queueChip.ChipType)
                        {
                            case ChipType.BPMChange:
                                {
                                    chipsState.CurrentParams.BPM = (float?)queueChip.Param ?? 150.0f;
                                }
                                break;
                            case ChipType.Scroll:
                                {
                                    chipsState.CurrentParams.Scroll = (Vector2?)queueChip.Param ?? new Vector2(1.0f, 0.0f);
                                }
                                break;
                            case ChipType.Measure:
                                {
                                    chipsState.CurrentParams.Measure = (Vector2?)queueChip.Param ?? new Vector2(4.0f, 4.0f);
                                }
                                break;
                        }

                        Chip chip = new Chip()
                        {
                            ChipType = queueChip.ChipType,
                            Params = chipsState.CurrentParams.Copy()
                        };

                        if (Chip.IsNote(chip.ChipType))
                        {
                            chipsState.CurrentParams.Time += step * (chip.Params.Measure.X / chip.Params.Measure.Y) / chip.Params.BPM;
                        }

                        courseState.ChipsData.Chips.Add(chip);

                    }
                    chipsState.ChipQueues.Clear();
                }
                else
                {
                    QueueChip queueChip = new QueueChip()
                    {
                        ChipType = CharToChipType(ch)
                    };

                    chipsState.ChipQueues.Add(queueChip);
                }
            }
        }
    }
}
