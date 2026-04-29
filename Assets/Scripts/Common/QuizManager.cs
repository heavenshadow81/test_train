using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class QuizManager : MonoBehaviour
    {
        public QuizData[] quizdata;
        public Quiz[] quiz;
        int QuizCount = 0;

        public Quiz[] SetQuiz()
        {
            if (KinectSkateManager.instance.stage == Stage.Stage1)
            {
                QuizCount = 4;
                quiz = new Quiz[QuizCount];
                for (int i = 0; i < quiz.Length; i++)
                {
                    quiz[i] = SetQuizLoop(0);
                }
            }
            else if (KinectSkateManager.instance.stage == Stage.Stage2)
            {
                QuizCount = 6;
                quiz = new Quiz[QuizCount];
                for (int i = 0; i < quiz.Length; i++)
                {
                    quiz[i] = SetQuizLoop(1);
                }
            }
            else if (KinectSkateManager.instance.stage == Stage.Stage3)
            {
                QuizCount = 8;
                quiz = new Quiz[QuizCount];
                for (int i = 0; i < 4; i++)
                {
                    quiz[i] = SetQuizLoop(0);
                }
                for (int i = 4; i < quiz.Length; i++)
                {
                    quiz[i] = SetQuizLoop(1);
                }
            }
            return quiz;
        }

        Quiz SetQuizLoop(int type)
        {
            int rand1 = 0, rand2 = 0, anser = 0;
            for(int i = 0; i<100; i++)
            {
                //무작위 퀴즈번호 및 정답 생성
                rand1 = Random.Range(0, QuizCount);
                rand2 = Random.Range(0, QuizCount);
                anser = Random.Range(1, 3);

                //두 퀴즈번호가 다를 때
                if (rand1 != rand2)
                {
                    //이미 생성한 퀴즈중에 중복되는 퀴즈가 있는지 확인하기
                    int cnt = 0;
                    for (int j = 0; j < quiz.Length; j++)
                    {
                        if (quiz[j] != null)
                        {
                            //같은 정답을 가진 문제가 있다면 중복된것.
                            /*  if (quizdata[rand1].ImageName == quiz[j].Select1 &&
                                  quizdata[rand2].ImageName == quiz[j].Select2 &&
                                  anser == quiz[j].Answer)*/
                            //새로 생성한 랜덤 정답이 1번일때 
                            if (anser == 1)
                            {
                                //이전에 생성한 퀴즈의 정답이 1번이라면 
                                if (quiz[j].Answer == 1)
                                {
                                    //새로 생성한 정답(1번)이름과 이전에 생성한 퀴즈의 정답(1번)을 비교.
                                    //이름이 같으면 중복된 문제이므로 cnt++
                                    if (quizdata[rand1].ImageName == quiz[j].Select1)
                                        cnt++;
                                }
                                //이전에 생성한 퀴즈의 정답이 2번이라면 
                                else if (quiz[j].Answer == 2)
                                {
                                    //새로 생성한 정답(1번)이름과 이전에 생성한 퀴즈의 정답(2번)을 비교.
                                    //이름이 같으면 중복된 문제이므로 cnt++
                                    if (quizdata[rand1].ImageName == quiz[j].Select2)
                                        cnt++;
                                }

                            }
                            //새로 생성한 랜덤 정답이 2번일때 
                            else if (anser == 2)
                            {
                                //이전에 생성한 퀴즈의 정답이 1번이라면 
                                if (quiz[j].Answer == 1)
                                {
                                    //새로 생성한 정답(2번)이름과 이전에 생성한 퀴즈의 정답(1번)을 비교.
                                    //이름이 같으면 중복된 문제이므로 cnt++
                                    if (quizdata[rand2].ImageName == quiz[j].Select1)
                                        cnt++;
                                }
                                //이전에 생성한 퀴즈의 정답이 2번이라면 
                                else if (quiz[j].Answer == 2)
                                {
                                    //새로 생성한 정답(2번)이름과 이전에 생성한 퀴즈의 정답(2번)을 비교.
                                    //이름이 같으면 중복된 문제이므로 cnt++
                                    if (quizdata[rand2].ImageName == quiz[j].Select2)
                                        cnt++;
                                }
                            }
                        }
                        
                    }
                    //중복된 퀴즈가 없다면 for문 종료
                    if (cnt == 0)
                        break;
                    else
                        i--;
                }                    
                else
                    i--;
            }
            string path = "";
            string[] name1 = quizdata[rand1].Imagepath.Split('#');
            string[] name2 = quizdata[rand2].Imagepath.Split('#');
            path = name1[0] + "#" + name1[1] + "&" + name2[1];
            //Debug.Log("GE 맞음? : "+name1[0]);
            Quiz tmp = new Quiz(0, quizdata[rand1].ImageName, quizdata[rand2].ImageName, anser, type, path);

            Debug.Log("중복되지 않는 퀴즈 생성 성공!");
            string anser_text = "";
            if (anser == 1)
                anser_text = name1[1];
            else
                anser_text = name2[1];
            Debug.Log("이미지 경로 : "+path + "\n정답 : " + anser_text + "\n세션 : " + type);
            return tmp;
        }
        public void GetQuizData(Stage stage)
        {
            quizdata = JsonHelper.Instance.Load(stage);
        }
    }

}
