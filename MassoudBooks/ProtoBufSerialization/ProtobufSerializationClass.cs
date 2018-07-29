using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassoudBooks.ProtoBufSerialization
{
    class ProtobufSerializationClass
    {
        public static void protoSave(Books books,string pathToSave)
        {
            MassoudBookSerialization.ToSerialize toSerialize = new MassoudBookSerialization.ToSerialize();
            foreach (var book in books.AllBooks)
            {
                MassoudBookSerialization.Book srBook = new MassoudBookSerialization.Book();
               
                MassoudBookSerialization.Book.Types.Catagories convertCatagory()
                {
                    switch (book.Catagory)
                    {
                        case Catagory.Any:
                            return MassoudBookSerialization.Book.Types.Catagories.AnyCatagory;
                        case Catagory.Math:
                            return MassoudBookSerialization.Book.Types.Catagories.Math;
                        case Catagory.GameDev:
                            return MassoudBookSerialization.Book.Types.Catagories.GameDev;
                        case Catagory.CSharp:
                            return MassoudBookSerialization.Book.Types.Catagories.Csharp;
                        case Catagory.Java:
                            return MassoudBookSerialization.Book.Types.Catagories.Java;
                        case Catagory.Cs:
                            return MassoudBookSerialization.Book.Types.Catagories.Cs;
                        case Catagory.Cpp:
                            return MassoudBookSerialization.Book.Types.Catagories.Cpp;
                        case Catagory.Emmbedded:
                            return MassoudBookSerialization.Book.Types.Catagories.Emmbedded;
                        case Catagory.PyAndML:
                            return MassoudBookSerialization.Book.Types.Catagories.PyAndMl;
                        case Catagory.Art:
                            return MassoudBookSerialization.Book.Types.Catagories.Art;
                        case Catagory.Android:
                            return MassoudBookSerialization.Book.Types.Catagories.Android;
                        default:
                            return MassoudBookSerialization.Book.Types.Catagories.AnyCatagory;
                    }
                }
                srBook.Catagory = convertCatagory();

                MassoudBookSerialization.Book.Types.ReadingStatus convertReadingStatus()
                {
                    switch (book.ReadingStatus)
                    {
                        case ReadingStatus.Any:
                            return MassoudBookSerialization.Book.Types.ReadingStatus.AnyReadingStatus;
                        case ReadingStatus.Reading:
                            return MassoudBookSerialization.Book.Types.ReadingStatus.Reading;
                        case ReadingStatus.WannaRead:
                            return MassoudBookSerialization.Book.Types.ReadingStatus.WannaRead;
                        case ReadingStatus.WannaRead2:
                            return MassoudBookSerialization.Book.Types.ReadingStatus.WannaRead2;
                        case ReadingStatus.YetToWannaRead:
                            return MassoudBookSerialization.Book.Types.ReadingStatus.YetToWannaRead;
                        case ReadingStatus.DelayedReading:
                            return MassoudBookSerialization.Book.Types.ReadingStatus.DelayedReading;
                        case ReadingStatus.Finished:
                            return MassoudBookSerialization.Book.Types.ReadingStatus.Finished;
                        case ReadingStatus.Dropped:
                            return MassoudBookSerialization.Book.Types.ReadingStatus.Dropped;
                        default:
                            return MassoudBookSerialization.Book.Types.ReadingStatus.AnyReadingStatus;
                    }
                }
                srBook.ReadingStatus = convertReadingStatus();

                foreach (var tag in book.BookTags)
                {
                    srBook.Tag.Add(tag);
                }
                srBook.Pages = book.PageNumber;
                toSerialize.AllBooks.Add(srBook);
            }
            using (Stream fileToSave = File.OpenWrite(pathToSave)) { 
                 toSerialize.WriteTo(fileToSave);
            }
        }
    }
}
