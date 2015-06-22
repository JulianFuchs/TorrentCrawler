using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TorrentCrawler
{
    class TorrentInfo
    {
        public enum Genre { Movie, TV, Music, Game, Application, Book, Anime, Other, XXX}

        public String infohash;
        public String torrentNameInMagnet;
        public String torrentName;
        public Genre genre;
        public int seeders;
        public int leechers;
        public String webLink;
        public String htmlWebpage;

        public TorrentInfo(String infohash, String nameFromMagnet,  String name, Genre genre, int seeders, int leechers, String webLink, String htmlWebpage)
        {
            this.infohash = infohash;
            this.torrentNameInMagnet = nameFromMagnet;
            this.torrentName = name;
            this.genre = genre;
            this.seeders = seeders;
            this.leechers = leechers;
            this.webLink = webLink;
            this.htmlWebpage = htmlWebpage;
        }

        public TorrentInfo()
        { }
        





    }
}
