using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Fool.Contracts;
using Fool.Domain;
using Fool.TextManagement.Models;
using Fool.TextManagement.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
namespace Fool.TextManagement.ViewModels
{
    public class TextManageViewModel : BindableBase
    {
        #region Fields
        private readonly IPublisherService mPublisherService;
        private readonly IRegionManager mRegionManager;
        private readonly ITextService mTextService;
        private CollectionViewSource mBooksView;
        private BookVm mCurrentBook;
        private ICommand mDeleteTextCommand;
        private ICommand mEditTextCommand;
        private ICommand mNewTextCommand;
        private ICommand mViewDetailsCommand;
        #endregion
        #region Properties
        public ObservableCollection<BookVm> Books { get; } = new ObservableCollection<BookVm>();
        public BookVm CurrentBook
        {
            get { return mCurrentBook; }
            set
            {
                mCurrentBook = value;
                RaisePropertyChanged();
            }
        }
        public CollectionViewSource BooksView
        {
            get
            {
                if(mBooksView == null)
                    mBooksView = new CollectionViewSource
                    {
                        Source = Books,
                        GroupDescriptions =
                        {
                            new PropertyGroupDescription("Publisher")
                        },
                        SortDescriptions =
                        {
                            new SortDescription("Publisher", ListSortDirection.Ascending)
                        }
                    };
                return mBooksView;
            }
        }
        public ICommand ViewDetailsCommand
        {
            get
            {
                return mViewDetailsCommand ??
                       (mViewDetailsCommand = new DelegateCommand<BookVm>(book => { CurrentBook = book; }));
            }
        }
        public ICommand NewTextCommand
        {
            get
            {
                return mNewTextCommand ??
                       (mNewTextCommand =
                           new DelegateCommand(
                               () =>
                               {
                                   mRegionManager.RequestNavigate(RegionNames.CONTENT, typeof(TextEditView).FullName);
                               }));
            }
        }
        public ICommand DeleteTextCommand
        {
            get
            {
                return mDeleteTextCommand ??
                       (mDeleteTextCommand = new DelegateCommand<TextVm>(text =>
                       {
                           var b = mTextService.RemoveText(text.Id);
                           if(b)
                           {
                               CurrentBook.Texts.Remove(text); 
                           } 
                       }));
            }
        }
        public ICommand EditTextCommand
        {
            get
            {
                return mEditTextCommand ?? (mEditTextCommand = new DelegateCommand<TextVm>(text =>
                       {
                           var parms = new NavigationParameters();
                           parms.Add("Id", text.Id);
                           parms.Add("publisher", CurrentBook.Publisher);
                           parms.Add("book", CurrentBook.Title); 
                           mRegionManager.RequestNavigate(RegionNames.CONTENT, typeof(TextEditView).FullName, parms);
                       }));
            }
        }
        #endregion
        #region Constructors
        public TextManageViewModel(IRegionManager regionManager, IPublisherService publisherService,
            ITextService textService)
        {
            mRegionManager = regionManager;
            mPublisherService = publisherService;
            mTextService = textService;
            var books = mPublisherService.GetPublishersAndBooks();
            foreach(var p in books)
                foreach(var b in p.Books)
                {
                    var book = new BookVm
                    {
                        Title = b.Title,
                        Id = b.Id,
                        Publisher = p.Title
                    };
                    Books.Add(book);
                }
            var texts = mTextService.GetTexts();
            foreach(var text in texts)
            {
                var book = Books.FirstOrDefault(a => a.Id == text.BookId);
                book?.Texts.Add(new TextVm
                {
                    Id = text.Id,
                    Title = text.Title
                });
            }
        }
        #endregion
    }
}