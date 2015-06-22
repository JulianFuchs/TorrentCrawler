using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO.MemoryMappedFiles;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;

namespace TorrentCrawler
{
    class Crawler
    {
        #region fields
        private String kickAss = "http://kickass.to";
        String kickAssLatest = "http://kickass.to/new/";
        private Queue<String> searchPages;
        private Queue<String> torrentSites;
        private int latestPagesMax;
        private int threadPoolCounter;
        ManualResetEvent waitForThreadPoolEvent;

        public Queue<TorrentInfo> torrentDataBase;
        private int enoughParticipants;

        private String myLock ="Data base access lock";

        private MySqlConnection mySqlConnection;

        private Queue<Exception> exceptionsDB;

        private int maxTries;

        private int newlyInsterted;
        private int updated;
        private int didntRespond;
        private int totalInspected;

        private DateTime startingTime;

        private int archiveSize;
        private int archiveProgressBarCurrent;
        private int archiveBlockCount;
        private bool[] progress;
        private int startingBlock;
        #endregion

        public Crawler()
        {
            searchPages = new Queue<string>();
            torrentSites = new Queue<string>();
            latestPagesMax = 400;
            threadPoolCounter = latestPagesMax; //necessary?
            enoughParticipants = 0;

            maxTries = 3;

            newlyInsterted = 0;
            updated = 0;
            didntRespond = 0;
            totalInspected = 0;

            startingTime = DateTime.Now;

            archiveProgressBarCurrent = 0; // TODO
            archiveBlockCount = 1498;
            archiveSize = 1498824695; // in Bytes
            progress = new bool[archiveBlockCount]; // should all be false, indexes go from 0 to archiveBlockCount-1
            startingBlock = 0;

            torrentDataBase = new Queue<TorrentInfo>();
            exceptionsDB = new Queue<Exception>();

            ThreadPool.SetMaxThreads(5, 5000);
            String connectionString = "Server=localhost;Uid=root;Pwd=wasihrwolle;Database=bachelor";
            mySqlConnection = new MySqlConnection(connectionString);

            mySqlConnection.Open();

        }

        public void determineProgress()
        {
            int index = 0;

            for (; index < archiveBlockCount; index++)
            {
                if (progress[index] == false)
                    break;
            }

            Program.blocksCompleted(index-1);
        }

        // Start Analyzing Archive Button calls this method
        public void startAnalyzingArchive()
        {
            Program.debug("Starting to analyze torrent archive...");

            // TODO: Fix blocks progress, updating 

            startingBlock = Program.getStartingBlockInput();
            for (int i = 0; i < startingBlock; i++)
            {
                progress[i] = true;
            }
            archiveProgressBarCurrent = startingBlock; // TODO: change
            determineProgress();
            Program.archiveProgressBarInitialization(archiveBlockCount, archiveProgressBarCurrent);

            analyzeKickAssArchive("C:\\Users\\Skaldik\\Documents\\ETH\\14HS\\BachelorThesis\\dailydump.txt");

            Program.debug("Analyzed all torrents in the archive");
            Program.debug(didntRespond + " sites didn't respond when asked " + maxTries + " times.");
        }

        // Start Analyzing Newest Button calls this method
        public void startAnalyzingNewset()
        {
            // TODO this should loop forever
            Program.debug("Starting to analyze " + latestPagesMax + " latest pages...");

            archiveProgressBarCurrent = 0; // TODO: change
            Program.archiveProgressBarInitialization(latestPagesMax, 0);

            analyzeNewest();
            Program.debug("Finished analyzing latest added torrents");
            Program.debug(didntRespond + " sites didn't respond when asked " + maxTries + " times.");
        }

        public void startTestingFunctions()
        {
            Program.debug("Starting to test functions..");
            //analyzeTorrentPage("https://kickass.so/the-hobbit-2014-battle-of-the-five-armies-dvdscr-xvid-ac3-hq-hive-cm8-t10046515.html"); // Test Movie
            //analyzeTorrentPage("https://kickass.so/gotham-s01e01-720p-hdtv-x264-dimension-eztv-t9615205.html"); // Test TV
            //analyzeTorrentPage("https://kickass.so/amon-amarth-deceiver-of-the-gods-limited-edition-2013-t9623889.html"); // Test Music
            //analyzeTorrentPage("https://kickass.so/far-cry-4-v-1-6-gold-edition-2014-repack-sgames-t10002976.html"); // Test Game
            //analyzeTorrentPage("https://kickass.so/horriblesubs-log-horizon-2-12-1080p-mkv-t9986154.html", 0); // Test Anime
            //analyzeTorrentPage("http://kickass.so/heyzo-0722-ichinose-luke-jav-uncensored-t9817749.html", 0);
            analyzeTorrentPage("https://kickass.to/150213-queen-bee-%E7%A9%B4%E3%81%AE%E5%A5%A5%E3%81%AE%E3%81%84%E3%81%84%E7%A7%98%E9%83%A8-%E3%81%A8%E3%81%93%E3%82%8D-2-%E6%B5%81%E4%B8%80%E6%9C%AC-mp4-t10219417.html",0);
            Program.debug("Finished testing functions");
        }

        private void analyzeKickAssArchive(String archiveLocation)
        {
            MemoryMappedFile memoryMappedArchive = MemoryMappedFile.CreateFromFile(archiveLocation, FileMode.Open);
            Encoding encoding = Encoding.GetEncoding("iso-8859-1"); // get extended Ascii Encoding
            
            int maxOffset = 1000000; int start = startingBlock * maxOffset + 1000; int overlap = 1000;

            waitForThreadPoolEvent = new ManualResetEvent(false);

            threadPoolCounter = archiveBlockCount; // TODO: change when you change the file, also, progress bar maximum value needs to change accordingly
            MemoryMappedViewAccessor memoryMappedAccessor;
            ArchiveBlockArguments archiveBlockArguments;

            

            for (int i = startingBlock; i<threadPoolCounter-1;i++)
            {
                memoryMappedAccessor = memoryMappedArchive.CreateViewAccessor(start - overlap, maxOffset + overlap);
                archiveBlockArguments = new ArchiveBlockArguments(encoding, memoryMappedAccessor, maxOffset + overlap, start-overlap,i);
                ThreadPool.QueueUserWorkItem(new WaitCallback(analyzeArchiveBlock), archiveBlockArguments);
                start += maxOffset;
            }

            // last block:
            memoryMappedAccessor = memoryMappedArchive.CreateViewAccessor(start - overlap, archiveSize-(start-overlap));
            archiveBlockArguments = new ArchiveBlockArguments(encoding, memoryMappedAccessor, maxOffset + overlap, start-overlap,threadPoolCounter);
            ThreadPool.QueueUserWorkItem(new WaitCallback(analyzeArchiveBlock), archiveBlockArguments);

            waitForThreadPoolEvent.WaitOne();
            
            //MemoryMappedViewAccessor memoryMappedAccessor = memoryMappedArchive.CreateViewAccessor(0, 1498824695);

        }

        private class ArchiveBlockArguments
        {
            public Encoding encoding;
            public MemoryMappedViewAccessor memoryMappedAccessor;
            public int maxOffset;
            public int startingPoint;
            public int blockNr;

            public ArchiveBlockArguments(Encoding encoding, MemoryMappedViewAccessor memoryMappedAccessor, int maxOffset, int startingPoint, int blockNr)
            {
                this.encoding = encoding;
                this.memoryMappedAccessor = memoryMappedAccessor;
                this.maxOffset = maxOffset;
                this.startingPoint = startingPoint;
                this.blockNr = blockNr;
            }
        }

        private void analyzeArchiveBlock(Object obj)
        {
            ArchiveBlockArguments arguments = (ArchiveBlockArguments) obj;
            Encoding encoding = arguments.encoding;
            MemoryMappedViewAccessor memoryMappedAccessor = arguments.memoryMappedAccessor;
            int maxOffset = arguments.maxOffset;
            int startingPoint = arguments.startingPoint;
            int blockNr = arguments.blockNr;

            Byte[] bytes = new Byte[maxOffset];
            for (int i = 0; i < maxOffset; i++)
            {
                Byte b = memoryMappedAccessor.ReadByte(i);
                bytes[i] = b;
            }

            //String s = System.Text.Encoding.ASCII.GetString(bts); //works as well
            String archiveBlock = encoding.GetString(bytes);

            //Program.debug("Current Thread Id: " + Thread.CurrentThread.ManagedThreadId + " is analyzing Archive Block of size: " + maxOffset);

            //Regex regex = new Regex("<div class=\"torrentname\">" + @"\s*" + "<a href=\"" + @"[^ ]*");
            Regex regex = new Regex("http://kickass.to/[^.]*.html");

            MatchCollection mc = regex.Matches(archiveBlock);

            foreach (Match m in mc)
            {
                analyzeTorrentPage(m.ToString(),0);
            }

            memoryMappedAccessor.Dispose();

            Program.debug("Thread " + Thread.CurrentThread.ManagedThreadId + " finished analyzing memory block starting from: " + startingPoint);
            Program.archiveProgressBarStep();
            progress[blockNr] = true;
            determineProgress();
            
            if (Interlocked.Decrement(ref threadPoolCounter) == 0)
                waitForThreadPoolEvent.Set();

        }

        private void analyzeNewest()
        {
            waitForThreadPoolEvent = new ManualResetEvent(false);

            threadPoolCounter = latestPagesMax;

            for (int i = 1; i <= latestPagesMax; i++)
            {
                String musicSearchPage = kickAssLatest + i;
                ThreadPool.QueueUserWorkItem(new WaitCallback(analyzeSearchPage), musicSearchPage);
            }

            waitForThreadPoolEvent.WaitOne();
        }

        private void analyzeSearchPage(Object searchPageObj)
        {
            String searchPage = (String)searchPageObj;

            Program.debug("Current Thread Id: " + Thread.CurrentThread.ManagedThreadId + " is analyzing: " + searchPage);

            String responseFromServer = makeHttpWebRequestWithGZip(searchPage);

            Regex regex = new Regex("<div class=\"torrentname\">" + @"\s*" + "<a href=\"" + @"[^ ]*");

            MatchCollection mc = regex.Matches(responseFromServer);
            Match match;

            foreach (Match m in mc)
            {
                String mtchStr = m.ToString();

                //regex = new Regex("/" + @"(\w|-|\d)*" + ".html");
                regex = new Regex("/" + "([^\"])*" + ".html");
                match = regex.Match(mtchStr);

                analyzeTorrentPage("http://kickass.to" + match.ToString(),0);
            }

            Program.archiveProgressBarStep();

            if (Interlocked.Decrement(ref threadPoolCounter) == 0)
                waitForThreadPoolEvent.Set();
        }

        private void analyzeTorrentPage(Object torrentPageObj, int nrTry)
        {
            totalInspected += 1;
            //string torrentPage = kickAss + (String)torrentPageObj;
            string torrentPage = (String)torrentPageObj;

            //Program.debug("Current Thread Id: " + Thread.CurrentThread.ManagedThreadId + " is analyzing: " + torrentPage);

            String responseFromServer = makeHttpWebRequestWithGZip(torrentPage); // TODO: handle no response from server. Error: returned ""

            // Some kickass sites do not response right away.
            // E.g.: https://kickass.so/c87-doujinshi-ice-sugar-song-ayano-stalk-twins-fucking-part1-original-t10105901.html
            // Just returns "" as response. So we just wait for some time and then try again. We try up to maxTries times.
            // If it still hasn't returned a real response, we skip this torrent and move on.

            if (responseFromServer == "")
                throw new Exception("Shit didn't work");

            else if (responseFromServer == "no return")
            {
                Thread.Sleep(300); // 30000 before, takes ages for long torrents because they all got removed by copyright owner, so we use a small timer only.
                if (nrTry < maxTries)
                {
                    analyzeTorrentPage(torrentPageObj, nrTry + 1);
                    totalInspected -= 1;
                }
                else
                {
                    didntRespond += 1;
                    // may be that the torrent got deleted, like: https://kickass.so/castingcouch-x-megan-rain-megan-mp4-t9829668.html
                    Program.torrentsNotResponding(didntRespond);
                }
            }

            else if (responseFromServer != "no return")
            {
                String infohash, torrentNameInMagnet, torrentName, kickAssAffiliation;
                TorrentInfo.Genre genre;
                int seeders, leechers, IMDbId;

                analyzeMagnet(responseFromServer, out infohash, out torrentNameInMagnet, out torrentName);

                analyzeGenre(responseFromServer, out genre);

                analyzeSeeders(responseFromServer, out seeders, out leechers);

                analyzeAffiliation(responseFromServer, genre, out IMDbId, out kickAssAffiliation);

                if (seeders + leechers >= enoughParticipants)
                {
                    //TorrentInfo torrentInfo = new TorrentInfo(infohash, torrentNameInMagnet, torrentName, genre, seeders, leechers, torrentPage, responseFromServer);

                    #region Single Insert Command
                    MySqlCommand insertCommand = mySqlConnection.CreateCommand();
                    insertCommand.CommandText = "INSERT INTO torrents(id,infohash,torrentname,genre,seeders,leechers,weblink,IMDbId,kickAssAffiliation) " + 
                        "VALUES(?id, ?infohash, ?torrentname, ?genre, ?seeders, ?leechers, ?weblink, ?IMDbId, ?kickAssAffiliation);";
                    insertCommand.Parameters.Add("?id", MySqlDbType.Int32).Value = 0; // let the db use it's auto increment feature
                    insertCommand.Parameters.Add("?infohash", MySqlDbType.VarChar).Value = infohash;
                    insertCommand.Parameters.Add("?torrentname", MySqlDbType.VarChar).Value = torrentNameInMagnet;
                    insertCommand.Parameters.Add("?genre", MySqlDbType.VarChar).Value = genre.ToString();
                    insertCommand.Parameters.Add("?seeders", MySqlDbType.Int32).Value = seeders;
                    insertCommand.Parameters.Add("?leechers", MySqlDbType.Int32).Value = leechers;
                    insertCommand.Parameters.Add("?weblink", MySqlDbType.VarChar).Value = torrentPage;
                    insertCommand.Parameters.Add("?IMDbId", MySqlDbType.Int32).Value = IMDbId;
                    insertCommand.Parameters.Add("?kickAssAffiliation", MySqlDbType.VarChar).Value = kickAssAffiliation;
                    
                    lock (myLock)
                    {
                        try
                        {
                            newlyInsterted += 1;
                            insertCommand.ExecuteNonQuery(); // if not unique entry: Exception
                        }
                        catch (Exception e)
                        {
                            // Assumption: We got an duplicate unique entry.
                            // Since the torrent is already in the db, we just need to update the seeders/leechers count to the new one. The rest 
                            // of the entries should be exactly the same.

                            updated += 1;
                            newlyInsterted -= 1;

                            // TODO: what if new torrent has more information? E.g. had IMDb-link. We should update our db. So we would need to check if
                            // the entry in the db already has aIMDb-link/kickAssAffiliation. If it doesn't have one of them, write that in the db.
                            
                            MySqlCommand updateCommand = mySqlConnection.CreateCommand();
                            updateCommand.CommandText = "UPDATE torrents SET seeders = ?seeders, leechers = ?leechers WHERE infohash = ?infohash;";
                            updateCommand.Parameters.Add("?seeders", MySqlDbType.Int32).Value = seeders;
                            updateCommand.Parameters.Add("?leechers", MySqlDbType.Int32).Value = leechers;
                            updateCommand.Parameters.Add("?infohash", MySqlDbType.VarChar).Value = infohash;

                            try
                            {
                                updateCommand.ExecuteNonQuery();
                            }
                            catch (Exception updateE)
                            {
                                // Should never ever get here (Throw excepttion TODO)
                                throw updateE;
                            }
                            finally
                            {
                                updateCommand.Dispose();
                            }

                        }
                        finally
                        {
                            insertCommand.Dispose();
                        }
                    }

                    #endregion

                    //Program.debug("This torrent will be considered in our recommendation.");
                }
                else
                {
                    //Program.debug("This torrent does not have enough seeders to be considered in our recommendation.");
                }
            }

            Program.torrentsInserted(newlyInsterted);
            Program.torrentsUpdated(updated);
            Program.torrentsNotResponding(didntRespond);
            Program.totalInspected(totalInspected);
            TimeSpan timeRunning = DateTime.Now - startingTime;
            Program.updateTimeRunning(timeRunning);

        }

        private String makeHttpWebRequestWithGZip(String webAddress)
        {
            String responseFromServer = "no return";
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(webAddress);

                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) // WebException unhandled: The operation timed out
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    responseFromServer = reader.ReadToEnd();
                }

                if (responseFromServer == "")
                    responseFromServer = "no return";

                return responseFromServer;
            }

            catch (Exception e)
            {
                //Program.debug("Current Thread Id: " + Thread.CurrentThread.ManagedThreadId + " failed making Webrequest: " + webAddress + ". Error: " + e.ToString());
                return responseFromServer;
            }
        }

        private void analyzeMagnet(String responseFromServer, out String infohash, out String torrentNameInMagnet, out String torrentName)
        {
            Regex magnetRegexBroad = new Regex("<a title=\"Magnet link\" href=\"([^\"]*)");

            Match magnetBroadMatch = magnetRegexBroad.Match(responseFromServer);

            String magnet = magnetRegexBroad.Replace(magnetBroadMatch.ToString(), "$1");

            //Program.debug("The Torrent has the magnet: ");
            //Program.debug(magnet);
            //Program.debug("");

            //TODO: read out the trackers. each tracker begins with: &tr=udp%3A%2F%2F"TRACKERADDR"%3A"PORTNR"%2Fannounce"IF NOT LAST ONE"

            Regex magnetDetailsRegex = new Regex("magnet:\\?xt=urn:btih:([^\\&]*)&dn=([^\\&]*)");
            Match magnetDetailsMatch = magnetDetailsRegex.Match(magnet);

            infohash = magnetDetailsMatch.Groups[1].ToString();
            torrentNameInMagnet = magnetDetailsMatch.Groups[2].ToString();

            Regex replacePlus = new Regex("\\+");
            torrentName = replacePlus.Replace(torrentNameInMagnet, " ");        
        }

        private void analyzeGenre(String responseFromServer, out TorrentInfo.Genre genre)
        {
            //TODO: [0-9]* has to match a number in site(?). On the bottom, the recommendations have the same regex. This only works because the match takes the
            // first match, which is the genre of the torrent we're looking at. If it wouldn't take the first match, the result would be wrong.
            Regex genreRegex = new Regex("<span id=\"cat_[0-9]*\"><strong><a href=\"/(\\w*)/\">(\\w*)<");
            Match genreMatch = genreRegex.Match(responseFromServer);

            String genreStr = genreMatch.Groups[2].ToString();

            switch (genreStr)
            {
                case "TV":
                    genre = TorrentInfo.Genre.TV;
                    break;
                case "Movies":
                    genre = TorrentInfo.Genre.Movie;
                    break;
                case "Anime":
                    genre = TorrentInfo.Genre.Anime;
                    break;
                case "Music":
                    genre = TorrentInfo.Genre.Music;
                    break;
                case "Games":
                    genre = TorrentInfo.Genre.Game;
                    break;
                case "Applications":
                    genre = TorrentInfo.Genre.Application;
                    break;
                case "Books":
                    genre = TorrentInfo.Genre.Book;
                    break;
                case "Other":
                    genre = TorrentInfo.Genre.Other;
                    break;
                case "XXX":
                    genre = TorrentInfo.Genre.XXX;
                    break;
                default:
                    genre = TorrentInfo.Genre.Other;
                    break;
            }
        }

        private void analyzeSeeders(String responseFromServer,out int seeders, out int leechers)
        {
            Regex seederRegex = new Regex("<div class=\"seedBlock\"><span class=\"seedLeachIcon\"></span>seeders: <strong itemprop=\"seeders\">([0-9]*)</strong></div>");
            Match seederMatch = seederRegex.Match(responseFromServer);

            Regex leecherRegex = new Regex("<div class=\"leechBlock\"><span class=\"seedLeachIcon\"></span>leechers: <strong itemprop=\"leechers\">([0-9]*)</strong></div>");
            Match leecherMatch = leecherRegex.Match(responseFromServer);

            String seederStr = seederMatch.Groups[1].ToString();
            String leecherStr = leecherMatch.Groups[1].ToString();

            try
            {
                seeders = Convert.ToInt32(seederStr);
                leechers = Convert.ToInt32(leecherStr); //Overflow exception in one case: http://kickass.to/akuma-subs-the-disappearance-of-haruhi-suzumiya-720p-t5083696.html
            }
            catch // if there is an overflow exception (which is an error by kickAss, no torrent has 4 billion leechers) we set the seeders/leechers to 0 and proceed
            {
                seeders = 0;
                leechers = 0;
            }
        }

        private void analyzeAffiliation(String responeFromServer, TorrentInfo.Genre genre, out int IMDbId, out String kickAssAffiliation)
        {
            // All these entries are optional, may be none of them exist
            //Regex affiliationRegex = new Regex("<div class=\"dataList\">([\\s\\S]*)</ul>");
            //Match affiliationMatch = affiliationRegex.Match(responeFromServer);
            //Match affiliationMatch = affiliationRegex.Match("<ul class=\"block overauto botmarg0\"><li><strong>Movie:</strong> <a href=\"/guardians-%of-the-galaxy-i2015381/\"><span>Guardians of the Galaxy</span></a></li></ul>");

            //String affiliationStr = affiliationMatch.Groups[1].ToString();

            Regex imdbRegex = new Regex("<li><strong>IMDb link:</strong> <a class=\"plain\" href=\"([^\"]*)\">([0-9]*)</a></li>");
            Match imdbMatch = imdbRegex.Match(responeFromServer);

            String IMDbIdStr = imdbMatch.Groups[2].ToString();

            // We encountered one torrent with IMDb Id > Int32.Max... This is not a valid IMDb Id, so the entry is bogus... With try we avoid the Int32-Parse Exception
            // and set the IMDbId to the default, i.e. 0.
            if (IMDbIdStr != "")
                try
                {
                    IMDbId = Int32.Parse(IMDbIdStr);
                }
                catch
                {
                    IMDbId = 0;
                }
            else
                IMDbId = 0;

            Regex affiliationRegex = new Regex("dummyInitialization");

            switch (genre)
            { //{ Movie, TV, Music, Game, Application, Book, Anime, Other, XXX}
                case TorrentInfo.Genre.Movie:
                    // Example: <li><strong>Movie:</strong> <a href="https://kickass.so/the-hobbit-there-and-back-again-i2310332/"><span>The Hobbit: There and Back Again</span></a></li>
                    affiliationRegex = new Regex("<li><strong>Movie:</strong> <a href=\"([^\"]*)\"><span>([^\"]*)</span></a></li>"); // Why does [\\w\\s]* not work???
                    break;
                case TorrentInfo.Genre.TV:
                    // Example: <li><a href="https://kickass.so/banshee-tv30823/">View all <strong>Banshee</strong> episodes</a></li>
                    affiliationRegex = new Regex("<li><a href=\"([^\"]*)\">View all <strong>([^\"]*)</strong> episodes</a></li>");
                    break;
                case TorrentInfo.Genre.Music:
                    // Example: <li><strong>Album: </strong><a href="https://kickass.so/search/amon%20amarth%20deceiver%20of%20the%20gods/"><span>Deceiver of the Gods</span></a>
                    affiliationRegex = new Regex("<li><strong>Album: </strong><a href=\"([^\"]*)\"><span>([^\"]*)</span></a>");
                    break;
                case TorrentInfo.Genre.Game:
                    // Example: <li><strong>Game:</strong> <a itemprop="url" href="https://kickass.so/far-cry-4-g46310/"><span itemprop="name">Far Cry 4</span></a></li>
                    affiliationRegex = new Regex("<li><strong>Game:</strong> <a itemprop=\"url\" href=\"([^\"]*)\"><span itemprop=\"name\">([^\"]*)</span></a></li>");
                    break;
                case TorrentInfo.Genre.Application:
                    // no affiliation
                    break;
                case TorrentInfo.Genre.Book:
                    // no affiliation
                    break;
                case TorrentInfo.Genre.Anime:
                    // Example: <li><a href="https://kickass.so/log-horizon-2014-a48494/">View all <strong>Log Horizon (2014)</strong> episodes</a></li>
                    affiliationRegex = new Regex("<li><a href=\"([^\"]*)\">View all <strong>([^\"]*)</strong> episodes</a></li>");
                    break;
                case TorrentInfo.Genre.Other:
                    // no affiliation
                    break;
                case TorrentInfo.Genre.XXX:
                    // no affiliation
                    break;
                default:
                    break;
            }

            Match affiliationMatch = affiliationRegex.Match(responeFromServer);

            kickAssAffiliation = affiliationMatch.Groups[1].ToString();
        }
    }
}
