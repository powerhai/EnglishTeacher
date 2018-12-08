using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AutoMapper;
using Fool.Contracts;
using Fool.TextManagement.Models;
using Microsoft.Win32;
using NAudio.Wave;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
namespace Fool.TextManagement.ViewModels
{
    public class TextEditViewModel : BindableBase, INavigationAware
    {
        #region Fields
        private readonly IAudioService mAudioService;
        private readonly bool mIsManual = false;
        private readonly IPublisherService mPublisherService;
        private readonly ISentenceTranslateService mSentenceTranslateService;
        private readonly IText2AudioService mText2AudioService;
        private readonly ITextService mTextService;
        private readonly ISentenceService mSentenceService;
        private readonly DispatcherTimer mTimer = new DispatcherTimer();
        private string mAudioFile;
        private AudioFileReader mAudioReader;
        private string mBook;
        private double mCurrent;
        private FlowDocument mDocument;
        private ICommand mFormatTextCommand;
        private bool mIsBusy;
        private bool mIsDocumentDisplay;
        private IRegionNavigationJournal mJournal;
        private long mLength;
        private WaveOutEvent mOutputDevice;
        private string mPublisher;
        private List<PublisherVm> mPublishers;
        private CollectionViewSource mSentenceViewSource;
        private string mText;
        private string mTitle;
        private bool mIsEditMode;
        #endregion
        #region Properties
        public double Current
        {
            get { return mCurrent; }
            set
            {
                mCurrent = value;
                RaisePropertyChanged();
            }
        }
        public int Id { get; set; }
        public string Title
        {
            get { return mTitle; }
            set
            {
                mTitle = value;
                RaisePropertyChanged();
            }
        }
        public string AudioFile
        {
            get { return mAudioFile; }
            set
            {
                mAudioFile = value;
                RaisePropertyChanged();
            }
        }
        public ICommand DisplaySourceCommand
        {
            get { return new DelegateCommand(() => { IsDocumentDisplay = false; }); }
        }
        public ICommand ChooseAudioFileCommand
        {
            get { return new DelegateCommand(ChooseAudioFile); }
        }
        public ICommand ResetRangeCommand
        {
            get { return new DelegateCommand(RestRange); }
        }
        public ICommand GoBackCommand
        {
            get { return new DelegateCommand(() =>
            {
                mJournal.GoBack();
            }); }
        }
        public ICommand GetAudioCommand
        {
            get { return new DelegateCommand(GetAudios); }
        }
        public ICommand PauseCommand
        {
            get { return new DelegateCommand(Pause); }
        }
        public bool IsBusy
        {
            get { return mIsBusy; }
            set
            {
                mIsBusy = value;
                RaisePropertyChanged();
            }
        }
        public ICommand SaveCommand
        {
            get { return new DelegateCommand(Save); }
        }
        public ICommand PlayCommand
        {
            get { return new DelegateCommand(Play); }
        }
        public ICommand DisplayDocumentCommand
        {
            get { return new DelegateCommand(() => { IsDocumentDisplay = true; }); }
        }
        public bool IsDocumentDisplay
        {
            get { return mIsDocumentDisplay; }
            set
            {
                mIsDocumentDisplay = value;
                RaisePropertyChanged();
            }
        }
        public string Text
        {
            get { return mText; }
            set
            {
                mText = value;
                RaisePropertyChanged();
                OnPropertyChanged("CanAnalyse");
            }
        }
        public long Length
        {
            get { return mLength; }
            set
            {
                mLength = value;
                RaisePropertyChanged();
            }
        }
        public List<PublisherVm> Publishers
        {
            get { return mPublishers; }
            set
            {
                mPublishers = value;
                RaisePropertyChanged();
            }
        }
        public IEnumerable<BookVm> Books
        {
            get
            {
                var books = new List<BookVm>();
                var p = Publishers.FirstOrDefault(a => a.Title.Equals(Publisher));
                return p.Books;
            }
        }
        public string Book
        {
            get { return mBook; }
            set
            {
                mBook = value;
                RaisePropertyChanged();
            }
        }
        public string Publisher
        {
            get { return mPublisher; }
            set
            {
                mPublisher = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Books");
            }
        }
        public CollectionViewSource SentenceViewSource
        {
            get
            {
                if(mSentenceViewSource == null)
                    mSentenceViewSource = new CollectionViewSource
                    {
                        Source = Sentences
                    };
                return mSentenceViewSource;
            }
        }
        public ObservableCollection<SentenceData> Sentences { get; } = new ObservableCollection<SentenceData>();
        public ICommand ResetBooksCommand
        {
            get { return new DelegateCommand<object>(o => { }); }
        }
        public FlowDocument Document
        {
            get { return mDocument; }
            set
            {
                mDocument = value;
                RaisePropertyChanged();
            }
        }
        public bool IsEditMode
        {
            get { return mIsEditMode; }
            set
            {
                mIsEditMode = value;
                RaisePropertyChanged();
            }
        }
        public ICommand FormatTextCommand
        {
            get { return mFormatTextCommand ?? (mFormatTextCommand = new DelegateCommand(FormatText)); }
        }
        public ICommand AnalyseTextCommand
        {
            get { return new DelegateCommand(AnalyseText); }
        }
        public ICommand TranslateCommand
        {
            get { return new DelegateCommand(TranslateSentences); }
        }
        public ICommand PlaySentenceCommand
        {
            get { return new DelegateCommand(PlaySentence); }
        }
        #endregion
        #region Constructors
        public TextEditViewModel(IPublisherService publisherService, IAudioService audioService,
            ISentenceTranslateService sentenceTranslateService, IText2AudioService text2AudioService , ITextService textService, ISentenceService sentenceService)
        {
            mPublisherService = publisherService;
            mAudioService = audioService;
            mSentenceTranslateService = sentenceTranslateService;
            mText2AudioService = text2AudioService;
            mTextService = textService;
            mSentenceService = sentenceService;
            mTimer.Tick += Callback;
            mTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            mTimer.Start();
            Init();
        }
        #endregion
        #region Public Methods
        public void Init()
        {
            var publishers = mPublisherService.GetPublishersAndBooks();
            Publishers = Mapper.Map<List<PublisherVm>>(publishers);
        }
        public void LoadText()
        {
            var txt = mTextService.GeText(Id);
            this.Title = txt.Title;
            this.Text = txt.Body;
            var sens = mSentenceService.GetSentencesOfText(Id);
            foreach(var sen in sens)
            {
                this.Sentences.Add(new SentenceData() { Chinese = sen.Chinese, Sentence = sen.English});
            }
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
        public void JumpTo(double br)
        {
            
        }
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            mJournal = navigationContext.NavigationService.Journal; 
            var id = navigationContext.Parameters["Id"];
            if(id != null)
            {
                Id = Convert.ToInt32(id);
                IsEditMode = true;
                LoadText();
                Publisher = navigationContext.Parameters["publisher"].ToString();
                Book = navigationContext.Parameters["book"].ToString();
            }
            else
            {
                IsEditMode = false;
            }
        }
        public void SetIsManual(bool b) {}
        #endregion
        #region Private or Protect Methods
        private void AnalyseText()
        {
            IsBusy = true;
            Sentences.Clear();
            var text = Text.Replace("\r\n", "\r");
            // 去除行首的发言人
            {
                var reg = new Regex(@"^.*:", RegexOptions.Multiline);
                text = reg.Replace(text, "");
            }
            var fkf = new[] {'.', '\"', '\r', '?', '!'};
            var cs = text.Split(fkf);
            foreach(var l in cs)
            {
                var s = l.Trim();
                if(string.IsNullOrEmpty(s))
                    continue;
                Sentences.Add(new SentenceData
                {
                    Sentence = l.Trim()
                });
            }
            InitRange();
            RaisePropertyChanged("CanSave");
            FindSign();
            MakeUtfText();
            IsBusy = false;
        }
        private void Callback(object sender, EventArgs eventArgs)
        {
            if(mIsManual)
                return;
            if((mAudioReader != null) && (mOutputDevice?.PlaybackState == PlaybackState.Playing))
                Current = mAudioReader.CurrentTime.TotalSeconds;
        }
        private void ChooseAudioFile()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".mp3",
                Filter = "mp3 |*.mp3"
            };
            var rv = dialog.ShowDialog();
            if(rv != true)
                return;
            AudioFile = dialog.FileName;
        }
        private void FindSign()
        {
            var curIndex = 0;
            for(var j = 0; j < Sentences.Count; j++)
            {
                var s = Sentences[j];
                var i = Text.IndexOf(s.Sentence, curIndex, StringComparison.Ordinal);
                var inx = i + s.Sentence.Length;
                if(inx >= Text.Length)
                    continue;
                var ch = Text[i + s.Sentence.Length];
                s.Sentence += ch;
                curIndex = i + s.Sentence.Length;
            }
        }
        private void FormatText()
        {
            var text = Text;
            //去除双引号
            //{
            //    var reg = new Regex("[\"“”。]");
            //    text = reg.Replace(this.Text, ".");
            //}
            // 去除行首的发言人
            //{
            //    var reg = new Regex(@"^.*:", RegexOptions.Multiline);
            //    text = reg.Replace(text, "");
            //}
            text = text.Replace('，', ',');
            text = text.Replace('。', '.');
            text = text.Replace(';', '.');
            //去除行首空格
            {
                var reg = new Regex(@"^\s*", RegexOptions.Multiline);
                text = reg.Replace(text, "");
            }

            //去除行尾空格
            {
                var reg = new Regex(@"\s*$", RegexOptions.Multiline);
                text = reg.Replace(text, "");
            }
            //去除多余空格
            {
                var reg = new Regex(@" {2,}");
                text = reg.Replace(text, " ");
            }
            {
                text = text.Replace(" ,", ",");
                text = text.Replace(" .", ".");
                text = text.Replace(" !", "!");
            }
            Text = text;
        }
        private void GetAudios()
        {
            IsBusy = true;
            foreach(var sen in Sentences)
            {
                sen.AudioFile = mText2AudioService.GetAudioFile(sen.Sentence);
                sen.IsBaiduAudio = true;
                sen.WithAudio = true;
            }
            IsBusy = false;
        }
        private void InitRange()
        {
            if((Length <= 0) || (Sentences.Count <= 0))
                return;
            var unitLen = Length / Sentences.Count;
            long start = 0;
            for(var i = 0; i < Sentences.Count; i++)
            {
                var cur = Sentences[i];
                cur.Start = start;
                cur.End = start + unitLen;
                start += unitLen;
                cur.WithAudio = true;
                cur.IsBaiduAudio = false;
            }
        }
        private void LoadAudio(SentenceData data)
        {
            var start = TimeSpan.FromSeconds(data.Start);
            var end = TimeSpan.FromSeconds(data.End);
            var len = end - start;
            var trimmed = new AudioFileReader(AudioFile).Skip(start).Take(len);
            var path = "abc.avi";
            WaveFileWriter.CreateWaveFile16(path, trimmed);
            mAudioService.UploadSentenceAudio(data.Sentence, path);
        }
        private void MakeUtfText()
        {
            var doc = new FlowDocument();
            var section = new Paragraph();
            doc.Blocks.Add(section);
            var curIndex = 0;
            for(var j = 0; j < Sentences.Count; j++)
            {
                var s = Sentences[j];
                var i = Text.IndexOf(s.Sentence, curIndex, StringComparison.Ordinal);
                if(i >= curIndex)
                {
                    var str = Text.Substring(curIndex, i - curIndex);
                    section.Inlines.Add(new Run(str));
                    var rk = new Run($"{j + 1}");
                    rk.Background = Brushes.Coral;
                    section.Inlines.Add(rk);
                    var rse = new Run(s.Sentence);
                    rse.Background = Brushes.PowderBlue;
                    section.Inlines.Add(rse);
                    curIndex = i + s.Sentence.Length;
                }
                if(j == Sentences.Count - 1)
                {
                    var str = Text.Substring(curIndex);
                    section.Inlines.Add(new Run(str));
                }
            }
            Document = doc;
        }
        private void Pause()
        {
            mOutputDevice?.Pause();
        }
        private void Play()
        {
            if(mOutputDevice == null)
                mOutputDevice = new WaveOutEvent();
            if(mAudioReader == null)
            {
                mAudioReader = new AudioFileReader(AudioFile);
                mOutputDevice.Init(mAudioReader);
                Length = (long)mAudioReader.TotalTime.TotalSeconds;
            }
            mOutputDevice.Play();
            RaisePropertyChanged("CanPlay");
            RaisePropertyChanged("CanPause");
            RaisePropertyChanged("CanSave");
            InitRange();
        }
        private void PlaySentence()
        {
            Pause();
            var cur = SentenceViewSource.View.CurrentItem as SentenceData;
            if(cur == null)
                return;
            if(cur.WithAudio == false)
                return;
            if(cur.IsBaiduAudio)
            {
                var trimmed = new AudioFileReader(cur.AudioFile);
                var outputDevice = new WaveOutEvent();
                outputDevice.Init(trimmed);
                outputDevice.Play();
            }
            else
            {
                var start = TimeSpan.FromSeconds(cur.Start);
                var end = TimeSpan.FromSeconds(cur.End);
                var len = end - start;
                var trimmed = new AudioFileReader(AudioFile).Skip(start).Take(len);
                var outputDevice = new WaveOutEvent();
                outputDevice.Init(trimmed);
                outputDevice.Play();                
            }

        }
        private void RestRange()
        {
            var cur = SentenceViewSource.View.CurrentItem as SentenceData;
            var curIdx = Sentences.IndexOf(cur);
            var alllen = Length - cur.End;
            var cot = (Sentences.Count - curIdx - 1);
            cot = cot != 0 ? cot : 1;
            var unitlen = alllen / cot;
            var start = cur.End;
            for(var i = curIdx + 1; i < Sentences.Count; i++)
            {
                var sen = Sentences[i];
                sen.Start = start;
                sen.End = start + unitlen;
                start += unitlen;
            }
        }
        private void SaveText()
        {
            if(IsEditMode)
            {
                 mTextService.UpdateText(this.Id, this.Title, this.Text, this.Publisher, this.Book);
            }
            else
            {
                 var rv = mTextService.CreateText(this.Book, this.Publisher, this.Title, this.Text);
                foreach(var sen in this.Sentences)
                {
                    
                }
            }
           
        }
        private void Save()
        {
            IsBusy = true;
            SaveText();
            //LoadAudio(SentenceViewSource.View.CurrentItem as SentenceData);
            IsBusy = false;
        }
        private void TranslateSentences()
        {
            IsBusy = true;
            foreach(var sen in Sentences)
                sen.Chinese = mSentenceTranslateService.Translate(sen.Sentence);
            IsBusy = false;
        }
        #endregion
    }
}